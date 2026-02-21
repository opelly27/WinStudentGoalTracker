#!/bin/bash
# dump-objects.sh

MYSQL="mysql"
BASE_OUTPUT_DIR="$(cd "$(dirname "$0")" && pwd)/Objects"
DATABASE="winstudentgoaltracker"

# Get password once
read -s -p "Enter MySQL password: " PASS
echo

# Connection parameters
CONN_PARAMS=(-h 10.66.66.1 -P 3309 -u root -p"$PASS")

# =============================================================================
# CONNECTION TEST
# =============================================================================
echo "Testing connection to MySQL..."
if ! $MYSQL "${CONN_PARAMS[@]}" -N -B --raw -e "SELECT 1" &>/dev/null; then
    echo ""
    echo "ERROR: Could not connect to MySQL."
    echo "Check your password, host (10.66.66.1), port (3309), and that the MySQL server is reachable."
    exit 1
fi
echo "Connection OK."

# Helper function to initialize output directory
initialize_output_dir() {
    local path="$1"
    if [ -d "$path" ]; then
        rm -f "$path"/*.sql 2>/dev/null
    else
        mkdir -p "$path"
    fi
    echo "$path"
}

# Helper function to run mysql and clean output
invoke_mysql_query() {
    local query="$1"
    $MYSQL "${CONN_PARAMS[@]}" -N -B --raw -e "$query" 2>/dev/null | tr -d '\r' | sed '/^$/d'
}

# =============================================================================
# TABLES (includes indexes; triggers handled separately)
# =============================================================================
TABLE_DIR=$(initialize_output_dir "$BASE_OUTPUT_DIR/tables")

echo ""
echo "Fetching table list..."
TABLES=()
while IFS= read -r line; do
    [ -n "$line" ] && TABLES+=("$line")
done < <(invoke_mysql_query "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = '$DATABASE' AND TABLE_TYPE = 'BASE TABLE'")
echo "Found ${#TABLES[@]} tables"

for table in "${TABLES[@]}"; do
    table=$(echo "$table" | xargs)
    [ -z "$table" ] && continue
    echo "  Dumping: $table"

    create_stmt=$(invoke_mysql_query "SHOW CREATE TABLE \`$DATABASE\`.\`$table\`" | cut -f2-)
    if [ -z "$create_stmt" ]; then
        echo "WARNING: Failed to dump table: $table" >&2
        continue
    fi

    # Get triggers for this table
    TRIGGERS=()
    while IFS= read -r line; do
        [ -n "$line" ] && TRIGGERS+=("$line")
    done < <(invoke_mysql_query "SELECT TRIGGER_NAME FROM information_schema.TRIGGERS WHERE EVENT_OBJECT_SCHEMA = '$DATABASE' AND EVENT_OBJECT_TABLE = '$table'")

    trigger_sql=""
    for trigger in "${TRIGGERS[@]}"; do
        trigger=$(echo "$trigger" | xargs)
        [ -z "$trigger" ] && continue

        trigger_def=$(invoke_mysql_query "SHOW CREATE TRIGGER \`$DATABASE\`.\`$trigger\`")
        if [ -n "$trigger_def" ]; then
            # SHOW CREATE TRIGGER: TriggerName<TAB>sql_mode<TAB>CreateStatement<TAB>...
            trigger_create=$(echo "$trigger_def" | cut -f3 | sed 's/\t.*$//')
            trigger_sql+=$'\n\nDELIMITER ;;\n'"$trigger_create"$';;\nDELIMITER ;'
        fi
    done

    printf '%s;%s\n' "$create_stmt" "$trigger_sql" > "$TABLE_DIR/$table.sql"
done

# =============================================================================
# VIEWS
# =============================================================================
VIEW_DIR=$(initialize_output_dir "$BASE_OUTPUT_DIR/views")

echo ""
echo "Fetching view list..."
VIEWS=()
while IFS= read -r line; do
    [ -n "$line" ] && VIEWS+=("$line")
done < <(invoke_mysql_query "SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = '$DATABASE'")
echo "Found ${#VIEWS[@]} views"

for view in "${VIEWS[@]}"; do
    view=$(echo "$view" | xargs)
    [ -z "$view" ] && continue
    echo "  Dumping: $view"

    create_stmt=$(invoke_mysql_query "SHOW CREATE VIEW \`$DATABASE\`.\`$view\`" | cut -f2 | sed 's/\t[^\t]*\t[^\t]*$//')
    if [ -z "$create_stmt" ]; then
        echo "WARNING: Failed to dump view: $view" >&2
        continue
    fi

    # Basic formatting to break long lines
    create_stmt=$(echo "$create_stmt" | sed \
        -e 's/ select /\nselect /g' \
        -e 's/ from /\nfrom /g' \
        -e 's/ left join /\nleft join /g' \
        -e 's/ inner join /\ninner join /g' \
        -e 's/ join /\njoin /g' \
        -e 's/ where /\nwhere /g' \
        -e 's/ and /\n  and /g' \
        -e 's/ or /\n  or /g')

    printf '%s;\n' "$create_stmt" > "$VIEW_DIR/$view.sql"
done

# =============================================================================
# FUNCTIONS
# =============================================================================
FUNCTION_DIR=$(initialize_output_dir "$BASE_OUTPUT_DIR/functions")

echo ""
echo "Fetching function list..."
FUNCTIONS=()
while IFS= read -r line; do
    [ -n "$line" ] && FUNCTIONS+=("$line")
done < <(invoke_mysql_query "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '$DATABASE' AND ROUTINE_TYPE = 'FUNCTION'")
echo "Found ${#FUNCTIONS[@]} functions"

for func in "${FUNCTIONS[@]}"; do
    func=$(echo "$func" | xargs)
    [ -z "$func" ] && continue
    echo "  Dumping: $func"

    # SHOW CREATE FUNCTION: FuncName<TAB>sql_mode<TAB>CreateStatement<TAB>...
    create_stmt=$(invoke_mysql_query "SHOW CREATE FUNCTION \`$DATABASE\`.\`$func\`" | cut -f3 | sed 's/END\t.*/END/')
    if [ -z "$create_stmt" ]; then
        echo "WARNING: Failed to dump function: $func" >&2
        continue
    fi

    printf 'DELIMITER ;;\n%s;;\nDELIMITER ;\n' "$create_stmt" > "$FUNCTION_DIR/$func.sql"
done

# =============================================================================
# PROCEDURES
# =============================================================================
PROCEDURE_DIR=$(initialize_output_dir "$BASE_OUTPUT_DIR/procedures")

echo ""
echo "Fetching procedure list..."
PROCS=()
while IFS= read -r line; do
    [ -n "$line" ] && PROCS+=("$line")
done < <(invoke_mysql_query "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '$DATABASE' AND ROUTINE_TYPE = 'PROCEDURE'")
echo "Found ${#PROCS[@]} procedures"

for proc in "${PROCS[@]}"; do
    proc=$(echo "$proc" | xargs)
    [ -z "$proc" ] && continue
    echo "  Dumping: $proc"

    # SHOW CREATE PROCEDURE: ProcName<TAB>sql_mode<TAB>CreateStatement<TAB>...
    create_stmt=$(invoke_mysql_query "SHOW CREATE PROCEDURE \`$DATABASE\`.\`$proc\`" | cut -f3 | sed 's/END\t.*/END/')
    if [ -z "$create_stmt" ]; then
        echo "WARNING: Failed to dump procedure: $proc" >&2
        continue
    fi

    printf 'DELIMITER ;;\n%s;;\nDELIMITER ;\n' "$create_stmt" > "$PROCEDURE_DIR/$proc.sql"
done

# =============================================================================
# SUMMARY
# =============================================================================
echo ""
echo "=========================================="
echo "Done! Schema exported to: $BASE_OUTPUT_DIR"
echo "  Tables:     ${#TABLES[@]}"
echo "  Views:      ${#VIEWS[@]}"
echo "  Functions:  ${#FUNCTIONS[@]}"
echo "  Procedures: ${#PROCS[@]}"
echo "=========================================="
