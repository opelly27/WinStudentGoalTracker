# dump-objects.ps1

$mysql = "C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe"
$baseOutputDir = Join-Path $PSScriptRoot "Objects"
$database = "winstudentgoaltracker"

# Get password once
$securePass = Read-Host "Enter MySQL password" -AsSecureString
$pass = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePass))

# Connection parameters
$connParams = @("-h", "10.66.66.1", "-P", "3309", "-u", "root", "-p$pass")

# =============================================================================
# CONNECTION TEST
# =============================================================================
Write-Host "Testing connection to MySQL..."
$connTest = & $mysql @connParams -N -B --raw -e "SELECT 1" 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: Could not connect to MySQL. Details:" -ForegroundColor Red
    Write-Host ($connTest | Out-String).Trim() -ForegroundColor Red
    Write-Host ""
    Write-Host "Check your password, host (10.66.66.1), port (3309), and that the MySQL server is reachable." -ForegroundColor Yellow
    exit 1
}
Write-Host "Connection OK." -ForegroundColor Green

# Helper function to initialize output directory
function Initialize-OutputDir {
    param([string]$path)
    if (Test-Path $path) {
        Remove-Item "$path\*.sql" -Force
    }
    else {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
    }
    return (Resolve-Path $path).Path
}

# Helper function to run mysql and clean output
function Invoke-MySqlQuery {
    param([string]$query)
    $raw = & $mysql @connParams -N -B --raw -e $query 2>$null
    if ($LASTEXITCODE -ne 0) { return $null }
    return @($raw) | ForEach-Object { $_ -replace "`r", "" } | ForEach-Object { $_.TrimEnd() } | Where-Object { $_ -ne "" }
}

# =============================================================================
# TABLES (includes indexes; triggers handled separately)
# =============================================================================
$tableDir = Initialize-OutputDir "$baseOutputDir\tables"

Write-Host "`nFetching table list..."
$tables = Invoke-MySqlQuery "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = '$database' AND TABLE_TYPE = 'BASE TABLE'"
$tables = @($tables | Where-Object { $_ -ne $null -and $_.Trim() -ne "" })
Write-Host "Found $($tables.Count) tables"

foreach ($table in $tables) {
    $table = $table.Trim()
    Write-Host "  Dumping: $table"
    
    $lines = @(Invoke-MySqlQuery "SHOW CREATE TABLE ``$database``.``$table``")
    if (-not $lines) {
        Write-Warning "Failed to dump table: $table"
        continue
    }
    
    # SHOW CREATE TABLE: TableName<TAB>CreateStatement
    $firstLineParts = $lines[0] -split "`t", 2
    $createStmt = $firstLineParts[1]
    
    if ($lines.Count -gt 1) {
        $createStmt += "`n" + ($lines[1..($lines.Count - 1)] -join "`n")
    }
    
    # Get triggers for this table
    $triggerLines = Invoke-MySqlQuery "SELECT TRIGGER_NAME FROM information_schema.TRIGGERS WHERE EVENT_OBJECT_SCHEMA = '$database' AND EVENT_OBJECT_TABLE = '$table'"
    $triggers = @($triggerLines | Where-Object { $_ -ne $null -and $_.Trim() -ne "" })
    
    $triggerSql = ""
    foreach ($trigger in $triggers) {
        $trigger = $trigger.Trim()
        $triggerDef = Invoke-MySqlQuery "SHOW CREATE TRIGGER ``$database``.``$trigger``"
        if ($triggerDef) {
            # SHOW CREATE TRIGGER: TriggerName<TAB>sql_mode<TAB>CreateStatement<TAB>...
            $triggerParts = $triggerDef[0] -split "`t", 4
            $triggerCreate = $triggerParts[2]
            if ($triggerDef.Count -gt 1) {
                $triggerCreate += "`n" + ($triggerDef[1..($triggerDef.Count - 1)] -join "`n")
            }
            # Remove trailing charset columns and normalize line endings
            $triggerCreate = $triggerCreate -replace "END\t.*$", "END" -replace "`r", "" -replace "\n{2,}", "`n"
            $triggerSql += "`n`nDELIMITER ;;`n$triggerCreate;;`nDELIMITER ;"
        }
    }
    
    $content = "$createStmt;$triggerSql`n"
    $filePath = Join-Path $tableDir "$table.sql"
    [System.IO.File]::WriteAllText($filePath, $content)
}

# =============================================================================
# VIEWS
# =============================================================================
$viewDir = Initialize-OutputDir "$baseOutputDir\views"

Write-Host "`nFetching view list..."
$views = Invoke-MySqlQuery "SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = '$database'"
$views = @($views | Where-Object { $_ -ne $null -and $_.Trim() -ne "" })
Write-Host "Found $($views.Count) views"

foreach ($view in $views) {
    $view = $view.Trim()
    Write-Host "  Dumping: $view"
    
    $lines = @(Invoke-MySqlQuery "SHOW CREATE VIEW ``$database``.``$view``")
    if (-not $lines) {
        Write-Warning "Failed to dump view: $view"
        continue
    }
    
    # SHOW CREATE VIEW: ViewName<TAB>CreateStatement<TAB>charset<TAB>collation
    $firstLineParts = $lines[0] -split "`t", 2
    $createStmt = $firstLineParts[1]
    
    if ($lines.Count -gt 1) {
        $createStmt += "`n" + ($lines[1..($lines.Count - 1)] -join "`n")
    }
    
    # Remove trailing charset columns if present
    $createStmt = $createStmt -replace "\t[^\t]+\t[^\t]+$", ""

    # Basic formatting to break long lines
    $createStmt = $createStmt -replace " select ", "`nselect "
    $createStmt = $createStmt -replace " from ", "`nfrom "
    $createStmt = $createStmt -replace " left join ", "`nleft join "
    $createStmt = $createStmt -replace " inner join ", "`ninner join "
    $createStmt = $createStmt -replace " join ", "`njoin "
    $createStmt = $createStmt -replace " where ", "`nwhere "
    $createStmt = $createStmt -replace " and ", "`n  and "
    $createStmt = $createStmt -replace " or ", "`n  or "

    $content = "$createStmt;`n"
    $filePath = Join-Path $viewDir "$view.sql"
    [System.IO.File]::WriteAllText($filePath, $content)
}

# =============================================================================
# FUNCTIONS
# =============================================================================
$functionDir = Initialize-OutputDir "$baseOutputDir\functions"

Write-Host "`nFetching function list..."
$functions = Invoke-MySqlQuery "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '$database' AND ROUTINE_TYPE = 'FUNCTION'"
$functions = @($functions | Where-Object { $_ -ne $null -and $_.Trim() -ne "" })
Write-Host "Found $($functions.Count) functions"

foreach ($func in $functions) {
    $func = $func.Trim()
    Write-Host "  Dumping: $func"
    
    $lines = @(Invoke-MySqlQuery "SHOW CREATE FUNCTION ``$database``.``$func``")
    if (-not $lines) {
        Write-Warning "Failed to dump function: $func"
        continue
    }
    
    # SHOW CREATE FUNCTION: FuncName<TAB>sql_mode<TAB>CreateStatement<TAB>...
    $firstLineParts = $lines[0] -split "`t", 3
    $createStmt = $firstLineParts[2]
    
    if ($lines.Count -gt 1) {
        $createStmt += "`n" + ($lines[1..($lines.Count - 1)] -join "`n")
    }
    
    $createStmt = $createStmt -replace "END\t.*$", "END"
    
    $content = "DELIMITER ;;`n$createStmt;;`nDELIMITER ;`n"
    $filePath = Join-Path $functionDir "$func.sql"
    [System.IO.File]::WriteAllText($filePath, $content)
}

# =============================================================================
# PROCEDURES
# =============================================================================
$procedureDir = Initialize-OutputDir "$baseOutputDir\procedures"

Write-Host "`nFetching procedure list..."
$procs = Invoke-MySqlQuery "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '$database' AND ROUTINE_TYPE = 'PROCEDURE'"
$procs = @($procs | Where-Object { $_ -ne $null -and $_.Trim() -ne "" })
Write-Host "Found $($procs.Count) procedures"

foreach ($proc in $procs) {
    $proc = $proc.Trim()
    Write-Host "  Dumping: $proc"
    
    $lines = @(Invoke-MySqlQuery "SHOW CREATE PROCEDURE ``$database``.``$proc``")
    if (-not $lines) {
        Write-Warning "Failed to dump procedure: $proc"
        continue
    }
    
    # SHOW CREATE PROCEDURE: ProcName<TAB>sql_mode<TAB>CreateStatement<TAB>...
    $firstLineParts = $lines[0] -split "`t", 3
    $createStmt = $firstLineParts[2]
    
    if ($lines.Count -gt 1) {
        $createStmt += "`n" + ($lines[1..($lines.Count - 1)] -join "`n")
    }
    
    $createStmt = $createStmt -replace "END\t.*$", "END"
    
    $content = "DELIMITER ;;`n$createStmt;;`nDELIMITER ;`n"
    $filePath = Join-Path $procedureDir "$proc.sql"
    [System.IO.File]::WriteAllText($filePath, $content)
}

# =============================================================================
# SUMMARY
# =============================================================================
$baseOutputDirFull = (Resolve-Path $baseOutputDir).Path
Write-Host "`n=========================================="
Write-Host "Done! Schema exported to: $baseOutputDirFull"
Write-Host "  Tables:     $($tables.Count)"
Write-Host "  Views:      $($views.Count)"
Write-Host "  Functions:  $($functions.Count)"
Write-Host "  Procedures: $($procs.Count)"
Write-Host "=========================================="

explorer.exe $baseOutputDirFull