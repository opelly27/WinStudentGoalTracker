using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace WinStudentGoalTracker.DataAccess;

public class StudentRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    public async Task<IEnumerable<dbStudent>> GetAllAsync()
    {
        using var db = Connection;
        return await db.QueryAsync<dbStudent>(
            "sp_Student_GetAll",
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

    public async Task<dbStudent?> InsertAsync(CreateStudentDto dto)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<dbStudent>(
            "sp_Student_Insert",
            new
            {
                p_id_student = dto.IdStudent.ToString(),
                p_id_program = dto.IdProgram?.ToString(),
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
                p_id_program = dto.IdProgram?.ToString(),
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
