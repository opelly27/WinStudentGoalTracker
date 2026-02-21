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

    public async Task<dbUser?> GetByIdAsync(int idUser)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbUser>(
            "sp_User_GetById",
            new { p_id_user = idUser },
            commandType: CommandType.StoredProcedure);
    }
}
