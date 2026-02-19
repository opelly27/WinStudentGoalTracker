// =============================================================================
// DummyGoalRepository.cs
// =============================================================================
// Hardcoded goal data. Goals belong to students and can only be created/edited
// by the PrimaryTeacher (enforced by the authorization handler, not here).
//
// This repository is intentionally "dumb" — it just stores and retrieves data.
// Authorization is handled elsewhere (in the handlers and controllers).
// This separation of concerns is important: the repository doesn't know or
// care about permissions. It just does CRUD.
// =============================================================================

using RolesAssignments.Models;

namespace RolesAssignments.Data;

public class DummyGoalRepository : IGoalRepository
{
    // Simulates the goals table. We use a static list so data persists
    // across requests during a single run of the app (but resets on restart).
    private static readonly List<Goal> _goals = new()
    {
        new Goal
        {
            Id = 1, StudentId = 101,
            Title = "Reading Fluency",
            Description = "Student will read 20 words per minute by end of semester.",
            CreatedByUserId = 1, CreatedAt = new DateTime(2025, 9, 15)
        },
        new Goal
        {
            Id = 2, StudentId = 101,
            Title = "Math Facts",
            Description = "Student will master addition facts to 10.",
            CreatedByUserId = 1, CreatedAt = new DateTime(2025, 9, 15)
        },
        new Goal
        {
            Id = 3, StudentId = 102,
            Title = "Social Skills",
            Description = "Student will initiate peer interactions 3 times per day.",
            CreatedByUserId = 1, CreatedAt = new DateTime(2025, 9, 16)
        },
    };

    // Counter for generating new IDs (simulates AUTO_INCREMENT)
    private static int _nextId = 4;

    public Task<IEnumerable<Goal>> GetByStudentId(int studentId)
    {
        var goals = _goals.Where(g => g.StudentId == studentId);

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // return await _db.QueryAsync<Goal>(
        //     @"SELECT id AS Id, student_id AS StudentId, title AS Title,
        //              description AS Description,
        //              created_by_user_id AS CreatedByUserId,
        //              created_at AS CreatedAt
        //       FROM goals
        //       WHERE student_id = @StudentId
        //       ORDER BY created_at",
        //     new { StudentId = studentId });

        return Task.FromResult(goals);
    }

    public Task<Goal?> GetById(int goalId)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == goalId);
        return Task.FromResult(goal);
    }

    public Task<int> Create(int studentId, CreateGoalRequest request, int createdByUserId)
    {
        var goal = new Goal
        {
            Id = _nextId++,
            StudentId = studentId,
            Title = request.Title,
            Description = request.Description,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
        _goals.Add(goal);

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // var goalId = await _db.ExecuteScalarAsync<int>(
        //     @"INSERT INTO goals (student_id, title, description, created_by_user_id, created_at)
        //       VALUES (@StudentId, @Title, @Description, @CreatedByUserId, NOW());
        //       SELECT LAST_INSERT_ID();",
        //     new { StudentId = studentId, request.Title, request.Description, CreatedByUserId = createdByUserId });

        return Task.FromResult(goal.Id);
    }

    public Task Update(int goalId, UpdateGoalRequest request, int updatedByUserId)
    {
        var goal = _goals.FirstOrDefault(g => g.Id == goalId);
        if (goal is not null)
        {
            goal.Title = request.Title;
            goal.Description = request.Description;
        }

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // await _db.ExecuteAsync(
        //     @"UPDATE goals
        //       SET title = @Title, description = @Description,
        //           updated_by_user_id = @UpdatedByUserId, updated_at = NOW()
        //       WHERE id = @GoalId",
        //     new { GoalId = goalId, request.Title, request.Description, UpdatedByUserId = updatedByUserId });

        return Task.CompletedTask;
    }
}
