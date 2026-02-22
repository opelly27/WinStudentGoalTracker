using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace WinStudentGoalTracker.DataAccess;

public class AuthRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    public async Task<Guid?> CreateRefreshTokenAsync(
        Guid refreshTokenId,
        Guid userId,
        Guid programId,
        string tokenHash,
        string tokenSalt,
        int expiresInSeconds,
        string? deviceInfo,
        string? userAgent)
    {
        using var db = Connection;
        var result = await db.QuerySingleOrDefaultAsync<Guid?>(
            "sp_RefreshToken_Create",
            new
            {
                p_id_refresh_token = refreshTokenId.ToString(),
                p_id_user = userId.ToString(),
                p_id_program = programId.ToString(),
                p_token_hash = tokenHash,
                p_token_salt = tokenSalt,
                p_expires_in_seconds = expiresInSeconds,
                p_device_info = deviceInfo,
                p_user_agent = userAgent
            },
            commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<dbRefreshToken?> GetRefreshTokenByIdAsync(Guid refreshTokenId)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbRefreshToken>(
            "sp_RefreshToken_GetById",
            new { p_id_refresh_token = refreshTokenId.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> RevokeRefreshTokenAsync(Guid refreshTokenId)
    {
        using var db = Connection;
        var rowsAffected = await db.QuerySingleOrDefaultAsync<int>(
            "sp_RefreshToken_Revoke",
            new { p_id_refresh_token = refreshTokenId.ToString() },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    public async Task<Guid?> ReplaceRefreshTokenAsync(
        Guid oldTokenId,
        Guid newTokenId,
        Guid userId,
        Guid programId,
        string tokenHash,
        string tokenSalt,
        int expiresInSeconds,
        string? deviceInfo,
        string? userAgent)
    {
        using var db = Connection;
        var result = await db.QuerySingleOrDefaultAsync<string?>(
            "sp_RefreshToken_Replace",
            new
            {
                p_old_token_id = oldTokenId.ToString(),
                p_id_refresh_token = newTokenId.ToString(),
                p_id_user = userId.ToString(),
                p_id_program = programId.ToString(),
                p_token_hash = tokenHash,
                p_token_salt = tokenSalt,
                p_expires_in_seconds = expiresInSeconds,
                p_device_info = deviceInfo,
                p_user_agent = userAgent
            },
            commandType: CommandType.StoredProcedure);
        return result != null ? Guid.Parse(result) : null;
    }
}
