using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using WinStudentGoalTracker.Models;

namespace WinStudentGoalTracker.DataAccess;

public class ReportPromptRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    // *****************************************************************
    // Returns all report prompts.
    // *****************************************************************
    public async Task<IEnumerable<ReportPromptResponse>> GetAllAsync()
    {
        using var db = Connection;
        return await db.QueryAsync<ReportPromptResponse>(
            "sp_ReportPrompt_GetAll",
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Returns a single report prompt by its ID, or null if not found.
    // *****************************************************************
    public async Task<ReportPromptResponse?> GetByIdAsync(Guid idReportPrompt)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<ReportPromptResponse>(
            "sp_ReportPrompt_GetById",
            new { p_id_report_prompt = idReportPrompt.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Returns a single report prompt by its reportname and program,
    // or null if not found.
    // *****************************************************************
    public async Task<ReportPromptResponse?> GetByReportnameAsync(string reportname, Guid programId)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<ReportPromptResponse>(
            "sp_ReportPrompt_GetByReportname",
            new
            {
                p_reportname = reportname,
                p_id_program = programId.ToString()
            },
            commandType: CommandType.StoredProcedure);
    }

    // *****************************************************************
    // Inserts a new report prompt and returns the created record.
    // *****************************************************************
    public async Task<ReportPromptResponse?> InsertAsync(CreateReportPromptDto dto)
    {
        var newId = Guid.NewGuid();
        using var db = Connection;
        await db.ExecuteAsync(
            "sp_ReportPrompt_Insert",
            new
            {
                p_id_report_prompt = newId.ToString(),
                p_id_program = dto.ProgramId,
                p_prompt = dto.Prompt,
                p_reportname = dto.Reportname
            },
            commandType: CommandType.StoredProcedure);

        return await GetByIdAsync(newId);
    }

    // *****************************************************************
    // Updates an existing report prompt. Returns true if a row was
    // affected, false otherwise.
    // *****************************************************************
    public async Task<bool> UpdateAsync(Guid idReportPrompt, UpdateReportPromptDto dto)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_ReportPrompt_Update",
            new
            {
                p_id_report_prompt = idReportPrompt.ToString(),
                p_prompt = dto.Prompt,
                p_reportname = dto.Reportname
            },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    // *****************************************************************
    // Deletes a report prompt by its ID. Returns true if a row was
    // affected, false otherwise.
    // *****************************************************************
    public async Task<bool> DeleteAsync(Guid idReportPrompt)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_ReportPrompt_Delete",
            new { p_id_report_prompt = idReportPrompt.ToString() },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }
}
