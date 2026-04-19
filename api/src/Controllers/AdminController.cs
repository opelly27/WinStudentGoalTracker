using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinStudentGoalTracker.BaseClasses;
using WinStudentGoalTracker.DataAccess;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.Services;

namespace WinStudentGoalTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : BaseController
{
    private readonly AdminRepository _adminRepo = new();

    // *****************************************************************
    // Returns the district ID for the current user by resolving the
    // program→district FK relationship. The JWT carries program_id
    // but not district_id; this method bridges that gap. This is a
    // deliberate design choice: the JWT stays lean, and we derive
    // the district from the program FK on each request.
    // *****************************************************************
    private async Task<(Guid districtId, ActionResult? error)> GetDistrictForCurrentUser(Guid programId)
    {
        var districtId = await _adminRepo.GetDistrictIdForProgramAsync(programId);
        if (!districtId.HasValue)
        {
            return (Guid.Empty, NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "District not found for the current program."
            }));
        }
        return (districtId.Value, null);
    }

    // ************************ Programs *************************

    // *****************************************************************
    // Returns all programs for the current user's district.
    // *****************************************************************
    [HttpGet("programs")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<List<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<List<object>>>> GetPrograms()
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null) return error;

        var (districtId, districtError) = await GetDistrictForCurrentUser(programId);
        if (districtError is not null) return districtError;

        var programs = await _adminRepo.GetProgramsByDistrictAsync(districtId);
        var result = programs.Select(p => new
        {
            programId = p.IdProgram,
            name = p.Name,
            description = p.Description,
            createdAt = p.CreatedAt
        }).ToList();

        return Ok(new ResponseResult<List<object>>
        {
            Success = true,
            Message = "Programs retrieved.",
            Data = result.Cast<object>().ToList()
        });
    }

    // *****************************************************************
    // Creates a new program under the current user's district.
    // *****************************************************************
    [HttpPost("programs")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<object>>> CreateProgram([FromBody] AdminCreateProgramDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Program name is required."
            });
        }

        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null) return error;

        var (districtId, districtError) = await GetDistrictForCurrentUser(programId);
        if (districtError is not null) return districtError;

        var newProgramId = Guid.NewGuid();
        var created = await _adminRepo.CreateProgramAsync(newProgramId, districtId, dto.Name, dto.Description);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = "Program created.",
            Data = new
            {
                programId = newProgramId,
                name = dto.Name,
                description = dto.Description
            }
        });
    }

    // *****************************************************************
    // Updates a program's name and description.
    // *****************************************************************
    [HttpPut("programs/{idProgram:guid}")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<object>>> UpdateProgram(Guid idProgram, [FromBody] AdminCreateProgramDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Program name is required."
            });
        }

        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null) return error;

        // Verify the program belongs to the user's district
        var (districtId, districtError) = await GetDistrictForCurrentUser(programId);
        if (districtError is not null) return districtError;

        var targetDistrictId = await _adminRepo.GetDistrictIdForProgramAsync(idProgram);
        if (!targetDistrictId.HasValue || targetDistrictId.Value != districtId)
        {
            return NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "Program not found in your district."
            });
        }

        await _adminRepo.UpdateProgramAsync(idProgram, dto.Name, dto.Description);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = "Program updated."
        });
    }

    // ************************ Users *************************

    // *****************************************************************
    // Returns all active users across programs in the user's district.
    // *****************************************************************
    [HttpGet("users")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<List<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<List<object>>>> GetUsers()
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null) return error;

        var (districtId, districtError) = await GetDistrictForCurrentUser(programId);
        if (districtError is not null) return districtError;

        var users = await _adminRepo.GetUsersByDistrictAsync(districtId);
        var result = users.Select(u => new
        {
            userId = u.IdUser,
            email = u.Email,
            name = u.Name,
            programId = u.IdProgram,
            programName = u.ProgramName,
            roleId = u.IdRole,
            roleName = u.RoleName,
            createdAt = u.CreatedAt
        }).ToList();

        return Ok(new ResponseResult<List<object>>
        {
            Success = true,
            Message = "Users retrieved.",
            Data = result.Cast<object>().ToList()
        });
    }

    // *****************************************************************
    // Creates a new user and assigns them to a program with a role.
    // District admins cannot assign the super_admin role.
    // *****************************************************************
    [HttpPost("users")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<object>>> CreateUser([FromBody] AdminCreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.ProgramId) ||
            string.IsNullOrWhiteSpace(dto.RoleId))
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Email, name, password, program, and role are required."
            });
        }

        if (!Guid.TryParse(dto.ProgramId, out var targetProgramId) ||
            !Guid.TryParse(dto.RoleId, out var roleId))
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Invalid program or role ID."
            });
        }

        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null) return error;

        // Verify the target program belongs to the user's district
        var (districtId, districtError) = await GetDistrictForCurrentUser(programId);
        if (districtError is not null) return districtError;

        var targetDistrictId = await _adminRepo.GetDistrictIdForProgramAsync(targetProgramId);
        if (!targetDistrictId.HasValue || targetDistrictId.Value != districtId)
        {
            return NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "Program not found in your district."
            });
        }

        // Prevent district_admin from assigning super_admin role
        var allRoles = await _adminRepo.GetAllRolesAsync();
        var selectedRole = allRoles.FirstOrDefault(r => r.IdRole == roleId);
        if (selectedRole == null)
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Invalid role."
            });
        }
        if (selectedRole.InternalName == UserRoles.SuperAdmin && role != UserRoles.SuperAdmin)
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Only super admins can assign the super admin role."
            });
        }

        // Check for duplicate email
        if (await _adminRepo.EmailExistsAsync(dto.Email))
        {
            return Ok(new ResponseResult<object>
            {
                Success = false,
                Message = "An account with this email already exists."
            });
        }

        // Create user
        var newUserId = Guid.NewGuid();
        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
        await _adminRepo.CreateUserAsync(newUserId, dto.Email, dto.Name, hash, salt);

        // Assign to program with role
        await _adminRepo.AssignUserToProgramAsync(newUserId, targetProgramId, roleId);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = "User created and assigned to program.",
            Data = new { userId = newUserId, email = dto.Email, name = dto.Name }
        });
    }

    // ************************ Roles *************************

    // *****************************************************************
    // Returns all available roles. Excludes super_admin for
    // non-super-admin users.
    // *****************************************************************
    [HttpGet("roles")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<List<object>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<List<object>>>> GetRoles()
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null) return error;

        var roles = await _adminRepo.GetAllRolesAsync();

        // District admins cannot see/assign the super_admin role
        if (role != UserRoles.SuperAdmin)
        {
            roles = roles.Where(r => r.InternalName != UserRoles.SuperAdmin);
        }

        var result = roles.Select(r => new
        {
            roleId = r.IdRole,
            name = r.Name,
            internalName = r.InternalName,
            description = r.Description
        }).ToList();

        return Ok(new ResponseResult<List<object>>
        {
            Success = true,
            Message = "Roles retrieved.",
            Data = result.Cast<object>().ToList()
        });
    }

    // ************************ Backup *************************

    // *****************************************************************
    // Exports the entire MySQL database as a .sql dump file using
    // MySqlBackup.NET. Runs in-process with no shell-out; uses a
    // single-transaction snapshot for InnoDB consistency (no locking).
    // Restricted to district/super admins.
    // *****************************************************************
    [HttpGet("backup")]
    [Authorize(Roles = $"{UserRoles.DistrictAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult BackupDatabase()
    {
        try
        {
            using var conn = new MySql.Data.MySqlClient.MySqlConnection(DatabaseManager.ConnectionString);
            conn.Open();

            using var cmd = new MySql.Data.MySqlClient.MySqlCommand { Connection = conn };
            var backup = new MySql.Data.MySqlClient.MySqlBackup(cmd);

            using var memoryStream = new MemoryStream();
            backup.ExportToMemoryStream(memoryStream);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"winstudentgoaltracker_backup_{timestamp}.sql";

            return File(memoryStream.ToArray(), "application/sql", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseResult<object>
            {
                Success = false,
                Message = $"Backup failed: {ex.Message}"
            });
        }
    }
}
