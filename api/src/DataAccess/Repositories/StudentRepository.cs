using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.Services;

namespace WinStudentGoalTracker.DataAccess;

public class StudentRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    public async Task<IEnumerable<StudentResponse>> GetMyStudentsAsync(Guid userId, Guid programId, string role)
    {
        using var db = Connection;
        using var multi = await db.QueryMultipleAsync(
            "sp_Student_GetWithAssignments",
            new { p_id_program = programId.ToString(), p_id_user = userId.ToString() },
            commandType: CommandType.StoredProcedure);

        var students = await multi.ReadAsync<StudentResponse>();
        var assignments = await multi.ReadAsync<dbUserStudent>();

        var myStudents = students.Where(s =>
            PermissionService.IsAllowed(role, EntityType.Student, PermissionAction.Read, assignments.Any(a => a.IdStudent == s.StudentId && a.IdUser == userId))
        );

        return myStudents;
    }

    public async Task<StudentResponse?> GetByIdAsync(Guid idStudent)
    {
        using var db = Connection;
        return await db.QuerySingleOrDefaultAsync<StudentResponse>(
            "sp_Student_GetById",
            new { p_id_student = idStudent.ToString() },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<StudentResponse?> InsertAsync(CreateStudentDto dto, Guid newStudentGuid, Guid programId, Guid userId)
    {
        using var db = Connection;
        await db.ExecuteAsync(
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

        return await GetByIdAsync(newStudentGuid);
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

    public async Task<bool> AddProgressEventAsync(Guid userId, AddProgressEventDto dto)
    {
        using var db = Connection;
        var row = await db.QuerySingleOrDefaultAsync(
            "sp_ProgressEvent_Insert",
            new
            {
                p_id_progress_event = Guid.NewGuid().ToString(),
                p_id_goal = dto.GoalId.ToString(),
                p_id_user_created = userId.ToString(),
                p_content = dto.Content,
                p_is_sensitive = dto.IsSensitive ? 1 : 0
            },
            commandType: CommandType.StoredProcedure);
        return row is not null;
    }

    public async Task<Guid?> GetStudentIdForGoalAsync(Guid idGoal)
    {
        using var db = Connection;
        var row = await db.QuerySingleOrDefaultAsync<dbGoalStudentRow>(
            "sp_Goal_GetById",
            new { p_id_goal = idGoal.ToString() },
            commandType: CommandType.StoredProcedure);

        return row?.StudentId;
    }

    public async Task<IEnumerable<ProgressEventResponse>> GetProgressEventsForGoalAsync(Guid idGoal)
    {
        using var db = Connection;
        var rows = await db.QueryAsync<dbProgressEventRow>(
            "sp_ProgressEvent_GetByGoalId",
            new
            {
                p_id_goal = idGoal.ToString()
            },
            commandType: CommandType.StoredProcedure);

        return rows.Select(r => new ProgressEventResponse
        {
            ProgressEventId = r.ProgressEventId,
            Content = r.Content,
            CreatedAt = r.CreatedAt,
            CreatedByName = r.CreatedByName
        });
    }

    public async Task<StudentGoalItem?> InsertGoalAsync(Guid idStudent, Guid userId, CreateGoalDto dto)
    {
        var newGoalId = Guid.NewGuid();
        using var db = Connection;
        var row = await db.QuerySingleOrDefaultAsync(
            "sp_Goal_Insert",
            new
            {
                p_id_goal = newGoalId.ToString(),
                p_id_goal_parent = dto.GoalParentId?.ToString(),
                p_id_student = idStudent.ToString(),
                p_id_user_created = userId.ToString(),
                p_title = dto.Title,
                p_description = dto.Description,
                p_category = dto.Category
            },
            commandType: CommandType.StoredProcedure);

        if (row is null) return null;

        return new StudentGoalItem
        {
            GoalId = newGoalId,
            GoalParentId = dto.GoalParentId,
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            ProgressEventCount = 0
        };
    }

    public async Task<StudentGoalSummary?> GetGoalSummaryAsync(Guid idStudent)
    {
        using var db = Connection;
        var rows = await db.QueryAsync<dbStudentGoalRow>(
            "sp_Goal_GetByStudentId",
            new { p_id_student = idStudent.ToString() },
            commandType: CommandType.StoredProcedure);

        var list = rows.ToList();
        if (list.Count == 0)
        {
            var student = await GetByIdAsync(idStudent);
            if (student is null) return null;

            return new StudentGoalSummary
            {
                StudentIdentifier = student.Identifier,
                Goals = []
            };
        }

        return new StudentGoalSummary
        {
            StudentIdentifier = list[0].StudentIdentifier,
            Goals = list.Select(r => new StudentGoalItem
            {
                GoalId = r.GoalId,
                GoalParentId = r.GoalParentId,
                Title = r.Title,
                Description = r.Description,
                Category = r.Category,
                ProgressEventCount = r.ProgressEventCount
            }).ToList()
        };
    }

}
