using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace WinStudentGoalTracker.DataAccess;

public class AdminRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    // *****************************************************************
    // Returns the district ID for a given program ID. Used to resolve
    // the user's district from their JWT program_id claim, since the
    // JWT does not carry district_id directly. The program→district
    // FK relationship is the source of truth.
    // *****************************************************************
    public async Task<Guid?> GetDistrictIdForProgramAsync(Guid programId)
    {
        using var db = Connection;
        var row = await db.QuerySingleOrDefaultAsync<dbProgramRow>(
            "sp_Program_GetById",
            new { p_id_program = programId.ToString() },
            commandType: CommandType.StoredProcedure);
        return row?.IdSchoolDistrict;
    }

    // *****************************************************************
    // Creates a new school district and returns the created row.
    // *****************************************************************
    public async Task<dynamic?> CreateDistrictAsync(Guid districtId, string name, string? contactEmail)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dynamic>(
            "sp_SchoolDistrict_Insert",
            new
            {
                p_id_school_district = districtId.ToString(),
                p_name = name,
                p_contact_email = contactEmail
            },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Creates a new user and returns the created row.
    // *****************************************************************
    public async Task<dynamic?> CreateUserAsync(Guid userId, string email, string name, string passwordHash, string passwordSalt)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dynamic>(
            "sp_User_Insert",
            new
            {
                p_id_user = userId.ToString(),
                p_email = email,
                p_name = name,
                p_password_hash = passwordHash,
                p_password_salt = passwordSalt
            },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Assigns a user to a program with a given role.
    // *****************************************************************
    public async Task<bool> AssignUserToProgramAsync(Guid userId, Guid programId, Guid roleId, bool isPrimary = true)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_UserProgram_Insert",
            new
            {
                p_id_user_program = Guid.NewGuid().ToString(),
                p_id_user = userId.ToString(),
                p_id_program = programId.ToString(),
                p_id_role = roleId.ToString(),
                p_is_primary = isPrimary ? 1 : 0
            },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    // *****************************************************************
    // Returns all programs belonging to a given district.
    // *****************************************************************
    public async Task<IEnumerable<dbProgramRow>> GetProgramsByDistrictAsync(Guid districtId)
    {
        using var db = Connection;
        return await db.QueryAsync<dbProgramRow>(
            "sp_Program_GetByDistrictId",
            new { p_id_school_district = districtId.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Creates a new program under a given district.
    // *****************************************************************
    public async Task<dbProgramRow?> CreateProgramAsync(Guid programId, Guid districtId, string name, string? description)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbProgramRow>(
            "sp_Program_Insert",
            new
            {
                p_id_program = programId.ToString(),
                p_id_school_district = districtId.ToString(),
                p_name = name,
                p_description = description
            },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Updates an existing program's name and description.
    // *****************************************************************
    public async Task<bool> UpdateProgramAsync(Guid programId, string name, string? description)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_Program_Update",
            new
            {
                p_id_program = programId.ToString(),
                p_name = name,
                p_description = description
            },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    // *****************************************************************
    // Returns all active users across all programs in a district.
    // *****************************************************************
    public async Task<IEnumerable<dbDistrictUserRow>> GetUsersByDistrictAsync(Guid districtId)
    {
        using var db = Connection;
        return await db.QueryAsync<dbDistrictUserRow>(
            "sp_User_GetByDistrictId",
            new { p_id_school_district = districtId.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Returns all available roles.
    // *****************************************************************
    public async Task<IEnumerable<dbRoleRow>> GetAllRolesAsync()
    {
        using var db = Connection;
        return await db.QueryAsync<dbRoleRow>(
            "sp_Role_GetAll",
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Returns a role by its internal_name (e.g. "district_admin").
    // Queries the role table directly since no SP exists for this.
    // *****************************************************************
    public async Task<dbRoleRow?> GetRoleByInternalNameAsync(string internalName)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbRoleRow>(
            "SELECT id_role AS IdRole, name AS Name, internal_name AS InternalName, description AS Description FROM role WHERE internal_name = @internalName",
            new { internalName });
    }

    // *****************************************************************
    // Checks if a user with the given email already exists.
    // *****************************************************************
    public async Task<bool> EmailExistsAsync(string email)
    {
        using var db = Connection;
        var count = await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM `user` WHERE email = @email",
            new { email });
        return count > 0;
    }
}
