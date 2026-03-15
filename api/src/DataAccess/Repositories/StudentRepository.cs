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
                p_next_iep_date = dto.NextIepDate
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
                p_next_iep_date = dto.NextIepDate
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
                p_description = dto.Description,
                p_category = dto.Category,
                p_baseline = dto.Baseline
            },
            commandType: CommandType.StoredProcedure);

        if (row is null) return null;

        return new StudentGoalItem
        {
            GoalId = newGoalId,
            GoalParentId = dto.GoalParentId,
            Description = dto.Description,
            Category = dto.Category,
            Baseline = dto.Baseline,
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
                Description = r.Description,
                Category = r.Category,
                Baseline = r.Baseline,
                ProgressEventCount = r.ProgressEventCount,
                BenchmarkCount = r.BenchmarkCount
            }).ToList()
        };
    }

    // *****************************************************************
    // Updates a goal's description, category, and baseline.
    // *****************************************************************
    public async Task<bool> UpdateGoalAsync(Guid goalId, UpdateGoalDto dto)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_Goal_Update",
            new
            {
                p_id_goal = goalId.ToString(),
                p_id_goal_parent = (string?)null,
                p_id_student = (string?)null,
                p_id_user_created = (string?)null,
                p_description = dto.Description,
                p_category = dto.Category,
                p_baseline = dto.Baseline
            },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

    // *****************************************************************
    // Returns all benchmarks for a student, grouped under a summary
    // with the student identifier. Returns null if student not found.
    // *****************************************************************
    public async Task<StudentBenchmarkSummary?> GetBenchmarkSummaryAsync(Guid idStudent)
    {
        using var db = Connection;
        var rows = await db.QueryAsync<dbStudentBenchmarkRow>(
            "sp_Benchmark_GetByStudentId",
            new { p_id_student = idStudent.ToString() },
            commandType: CommandType.StoredProcedure);

        var list = rows.ToList();
        if (list.Count == 0)
        {
            var student = await GetByIdAsync(idStudent);
            if (student is null) return null;

            return new StudentBenchmarkSummary
            {
                StudentIdentifier = student.Identifier,
                Benchmarks = []
            };
        }

        return new StudentBenchmarkSummary
        {
            StudentIdentifier = list[0].StudentIdentifier,
            Benchmarks = list.Select(r => new StudentBenchmarkItem
            {
                BenchmarkId = r.BenchmarkId,
                GoalId = r.GoalId,
                GoalCategory = r.GoalCategory,
                Benchmark = r.Benchmark,
                ShortName = r.ShortName,
                CreatedByName = r.CreatedByName,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList()
        };
    }

    // *****************************************************************
    // Inserts a new benchmark and returns the created benchmark item.
    // *****************************************************************
    public async Task<StudentBenchmarkItem?> InsertBenchmarkAsync(Guid goalId, Guid userId, CreateBenchmarkDto dto)
    {
        var newId = Guid.NewGuid();
        using var db = Connection;
        var row = await db.QuerySingleOrDefaultAsync(
            "sp_Benchmark_Insert",
            new
            {
                p_id_benchmark = newId.ToString(),
                p_id_goal = goalId.ToString(),
                p_id_user_created = userId.ToString(),
                p_benchmark = dto.Benchmark,
                p_short_name = dto.ShortName
            },
            commandType: CommandType.StoredProcedure);

        if (row is null) return null;

        return new StudentBenchmarkItem
        {
            BenchmarkId = newId,
            GoalId = goalId,
            Benchmark = dto.Benchmark,
            ShortName = dto.ShortName,
            CreatedAt = DateTime.UtcNow
        };
    }

    // *****************************************************************
    // Updates a benchmark's text and returns whether rows were affected.
    // *****************************************************************
    public async Task<bool> UpdateBenchmarkAsync(Guid benchmarkId, UpdateBenchmarkDto dto)
    {
        using var db = Connection;
        var rowsAffected = await db.ExecuteScalarAsync<int>(
            "sp_Benchmark_Update",
            new
            {
                p_id_benchmark = benchmarkId.ToString(),
                p_benchmark = dto.Benchmark,
                p_short_name = dto.ShortName
            },
            commandType: CommandType.StoredProcedure);
        return rowsAffected > 0;
    }

}
