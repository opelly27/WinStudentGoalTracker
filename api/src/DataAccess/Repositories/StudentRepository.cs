using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.Services;

namespace WinStudentGoalTracker.DataAccess;

public class StudentRepository
{
    private IDbConnection Connection => new MySqlConnection(DatabaseManager.ConnectionString);

    // *****************************************************************
    // Returns students visible to the current user. When scope is
    // "all", returns every student in the program enriched with the
    // owning user's name. Otherwise returns only the user's own.
    // *****************************************************************
    public async Task<IEnumerable<StudentResponse>> GetMyStudentsAsync(Guid userId, Guid programId, string role, string? scope = null)
    {
        using var db = Connection;
        using var multi = await db.QueryMultipleAsync(
            "sp_Student_GetWithAssignments",
            new { p_id_program = programId.ToString(), p_id_user = userId.ToString(), p_scope = scope },
            commandType: CommandType.StoredProcedure);

        var students = (await multi.ReadAsync<StudentResponse>()).ToList();
        var assignments = (await multi.ReadAsync<dbUserStudent>()).ToList();

        // When scope is "all", return every student in the program.
        // Otherwise, return only students assigned to the current user.
        var filtered = scope == "all"
            ? students
            : students.Where(s => assignments.Any(a => a.IdStudent == s.StudentId && a.IdUser == userId)).ToList();

        // Enrich each student with the primary owner's display name and ownership flag.
        foreach (var student in filtered)
        {
            var owner = assignments.FirstOrDefault(a => a.IdStudent == student.StudentId && (a.IsPrimary == true));
            owner ??= assignments.FirstOrDefault(a => a.IdStudent == student.StudentId);
            student.OwnerName = owner?.OwnerName;
            student.IsMine = assignments.Any(a => a.IdStudent == student.StudentId && a.IdUser == userId);
        }

        return filtered;
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

    // *****************************************************************
    // Saves a progress event (insert or update) and syncs benchmark
    // associations in a single stored procedure call.
    // *****************************************************************
    public async Task<Guid?> SaveProgressEventAsync(
        Guid progressEventId, Guid goalId, Guid userId,
        string? content, bool isNew, List<Guid>? benchmarkIds)
    {
        var idsCsv = benchmarkIds is { Count: > 0 }
            ? string.Join(",", benchmarkIds.Select(id => id.ToString()))
            : null;

        using var db = Connection;
        var row = await db.QuerySingleOrDefaultAsync<dynamic>(
            "sp_ProgressEvent_Save",
            new
            {
                p_id_progress_event = progressEventId.ToString(),
                p_id_goal = goalId.ToString(),
                p_id_user_created = userId.ToString(),
                p_content = content,
                p_is_sensitive = 0,
                p_is_new = isNew ? 1 : 0,
                p_benchmark_ids = idsCsv
            },
            commandType: CommandType.StoredProcedure);

        if (row is null) return null;
        return row.progressEventId is Guid g ? g : Guid.Parse((string)row.progressEventId);
    }

    // *****************************************************************
    // Returns the benchmark IDs associated with a progress event.
    // *****************************************************************
    public async Task<List<Guid>> GetBenchmarkIdsForEventAsync(Guid progressEventId)
    {
        using var db = Connection;
        var rows = await db.QueryAsync<dynamic>(
            "sp_ProgressEventBenchmark_GetByEventId",
            new { p_id_progress_event = progressEventId.ToString() },
            commandType: CommandType.StoredProcedure);

        return rows.Select(r => r.benchmarkId is Guid g ? g : Guid.Parse((string)r.benchmarkId)).ToList();
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
                p_baseline = dto.Baseline,
                p_target_completion_date = dto.TargetCompletionDate
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
            TargetCompletionDate = dto.TargetCompletionDate,
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
                TargetCompletionDate = r.TargetCompletionDate,
                CloseDate = r.CloseDate,
                Achieved = r.Achieved,
                CloseNotes = r.CloseNotes,
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
                p_baseline = dto.Baseline,
                p_target_completion_date = dto.TargetCompletionDate,
                p_close_date = dto.CloseDate,
                p_achieved = dto.Achieved,
                p_close_notes = dto.CloseNotes
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

    // *****************************************************************
    // Returns a full student profile: student card, goals, benchmarks,
    // progress events, and benchmark/event associations in one call.
    // *****************************************************************
    public async Task<StudentFullProfileResponse?> GetFullProfileAsync(Guid idStudent)
    {
        using var db = Connection;
        using var multi = await db.QueryMultipleAsync(
            "sp_Student_GetFullProfile",
            new { p_id_student = idStudent.ToString() },
            commandType: CommandType.StoredProcedure);

        // Result set 1: Student card
        var student = await multi.ReadSingleOrDefaultAsync<StudentResponse>();
        if (student is null) return null;

        // Result set 2: Goals
        var goalRows = (await multi.ReadAsync<dbStudentGoalRow>()).ToList();

        // Result set 3: Benchmarks
        var benchmarkRows = (await multi.ReadAsync<dbStudentBenchmarkRow>()).ToList();

        // Result set 4: Progress events
        var eventRows = (await multi.ReadAsync<dbProgressEventWithGoalRow>()).ToList();

        // Result set 5: Benchmark/event associations
        var linkRows = (await multi.ReadAsync<dbProgressEventBenchmarkRow>()).ToList();

        return new StudentFullProfileResponse
        {
            Student = student,
            Goals = goalRows.Select(r => new StudentGoalItem
            {
                GoalId = r.GoalId,
                GoalParentId = r.GoalParentId,
                Description = r.Description,
                Category = r.Category,
                Baseline = r.Baseline,
                TargetCompletionDate = r.TargetCompletionDate,
                CloseDate = r.CloseDate,
                Achieved = r.Achieved,
                CloseNotes = r.CloseNotes,
                ProgressEventCount = r.ProgressEventCount,
                BenchmarkCount = r.BenchmarkCount
            }).ToList(),
            Benchmarks = benchmarkRows.Select(r => new StudentBenchmarkItem
            {
                BenchmarkId = r.BenchmarkId,
                GoalId = r.GoalId,
                GoalCategory = r.GoalCategory,
                Benchmark = r.Benchmark,
                ShortName = r.ShortName,
                CreatedByName = r.CreatedByName,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList(),
            ProgressEvents = eventRows.Select(r => new ProgressEventWithGoalResponse
            {
                ProgressEventId = r.ProgressEventId,
                GoalId = r.GoalId,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                CreatedByName = r.CreatedByName
            }).ToList(),
            ProgressEventBenchmarks = linkRows.Select(r => new ProgressEventBenchmarkLink
            {
                ProgressEventId = r.ProgressEventId,
                BenchmarkId = r.BenchmarkId
            }).ToList()
        };
    }

    // *****************************************************************
    // Returns a full progress report for a student within the given
    // date range. Calls sp_ProgressReport_GetByStudentId which returns
    // two result sets: goals and progress events with benchmark names.
    // *****************************************************************
    public async Task<StudentProgressReportResponse?> GetProgressReportAsync(
        Guid studentId, DateTime fromDate, DateTime toDate, string? goalIds = null)
    {
        var student = await GetByIdAsync(studentId);
        if (student is null) return null;

        using var db = Connection;
        using var multi = await db.QueryMultipleAsync(
            "sp_ProgressReport_GetByStudentId",
            new
            {
                p_id_student = studentId.ToString(),
                p_from_date = fromDate.ToString("yyyy-MM-dd"),
                p_to_date = toDate.ToString("yyyy-MM-dd"),
                p_goal_ids = goalIds
            },
            commandType: CommandType.StoredProcedure);

        var goalRows = (await multi.ReadAsync<dbProgressReportGoalRow>()).ToList();
        var eventRows = (await multi.ReadAsync<dbProgressReportRow>()).ToList();

        var eventsByGoal = eventRows.GroupBy(e => e.GoalId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return new StudentProgressReportResponse
        {
            StudentIdentifier = student.Identifier,
            Goals = goalRows.Select(g => new ProgressReportGoal
            {
                GoalId = g.GoalId,
                Category = g.Category,
                Description = g.Description,
                ProgressEvents = eventsByGoal.TryGetValue(g.GoalId, out var events)
                    ? events.Select(e => new ProgressReportEvent
                    {
                        ProgressEventId = e.ProgressEventId,
                        Content = e.Content,
                        CreatedAt = e.CreatedAt,
                        BenchmarkNames = e.BenchmarkNames
                    }).ToList()
                    : []
            }).ToList()
        };
    }

}
