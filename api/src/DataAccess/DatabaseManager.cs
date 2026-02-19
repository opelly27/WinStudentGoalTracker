using Dapper;
using WinStudentGoalTracker.Api.Configuration;

namespace WinStudentGoalTracker.DataAccess;

public static class DatabaseManager
{
    static DatabaseManager()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static string ConnectionString =>
        ConfigHelper.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new MissingFieldException("DefaultConnection not configured.");
}
