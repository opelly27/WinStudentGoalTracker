using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace WinStudentGoalTracker.DataAccess;

public class UserRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    public async Task<dbUser?> GetByEmailAsync(string email)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbUser>(
            "sp_User_GetByEmail",
            new { p_email = email },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dbUser?> GetByIdAsync(Guid idUser)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbUser>(
            "sp_User_GetById",
            new { p_id_user = idUser.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dbProgramUser?> GetByIdWithProgramAsync(Guid idUser, Guid idProgram)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbProgramUser>(
            "sp_User_GetById_WithProgram",
            new { p_id_user = idUser.ToString(), p_id_program = idProgram.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<dbUserProgram>> GetProgramsForUserIdAsync(Guid idUser)
    {
        using var db = Connection;
        return await db.QueryAsync<dbUserProgram>(
            "sp_UserPrograms_GetByUserId",
            new { p_id_user = idUser.ToString() },
            commandType: CommandType.StoredProcedure);
    }
}
