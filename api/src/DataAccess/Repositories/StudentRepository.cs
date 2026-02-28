using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using WinStudentGoalTracker.Models;

namespace WinStudentGoalTracker.DataAccess;

public class StudentRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    public async Task<IEnumerable<dbStudent>> GetMyStudentsAsync(Guid userId, Guid programId, string role)
    {
        return role switch
        {
            UserRoles.Teacher or UserRoles.ProgramAdmin =>
                await GetStudentsByProgramAsync(programId),
            UserRoles.Paraeducator =>
                await GetAssignedStudentsAsync(userId, programId),
            _ => Enumerable.Empty<dbStudent>()
        };
    }

    public async Task<IEnumerable<dbStudent>> GetStudentsByProgramAsync(Guid programId)
    {
        using var db = Connection;
        return await db.QueryAsync<dbStudent>(
            "sp_Student_GetByProgram",
            new { p_id_program = programId.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    private async Task<IEnumerable<dbStudent>> GetAssignedStudentsAsync(Guid userId, Guid programId)
    {
        using var db = Connection;
        return await db.QueryAsync<dbStudent>(
            "sp_Student_GetByUserAndProgram",
            new { p_id_user = userId.ToString(), p_id_program = programId.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dbStudent?> GetByIdAsync(Guid idStudent)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbStudent>(
            "sp_Student_GetById",
            new { p_id_student = idStudent.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<dbStudent?> InsertAsync(CreateStudentDto dto, Guid newStudentGuid, Guid programId, Guid userId)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbStudent>(
            "sp_Student_Insert",
            new
            {
                p_id_student = newStudentGuid.ToString(),
                p_id_program = programId,
                p_id_user = userId.ToString(),
                p_identifier = dto.Identifier,
                p_program_year = dto.ProgramYear,
                p_enrollment_date = dto.EnrollmentDate,
                p_expected_grad = dto.ExpectedGrad
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateAsync(Guid idStudent, UpdateStudentDto dto)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_Student_Update",
            new
            {
                p_id_student = idStudent.ToString(),
                p_identifier = dto.Identifier,
                p_program_year = dto.ProgramYear,
                p_enrollment_date = dto.EnrollmentDate,
                p_expected_grad = dto.ExpectedGrad
            },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid idStudent)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_Student_Delete",
            new { p_id_student = idStudent.ToString() },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }
}
