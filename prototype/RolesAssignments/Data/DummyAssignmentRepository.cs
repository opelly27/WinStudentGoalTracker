// =============================================================================
// DummyAssignmentRepository.cs
// =============================================================================
// This is the HARDCODED version of the assignment repository.
// It simulates what the database would return, without needing a real database.
//
// THE DEMO DATA:
// ┌─────────┬───────────────┬────────────┬───────────────────┐
// │ User ID │ Name          │ Student ID │ Assignment Type   │
// ├─────────┼───────────────┼────────────┼───────────────────┤
// │ 1       │ Ms. Rivera    │ 101        │ PrimaryTeacher    │
// │ 1       │ Ms. Rivera    │ 102        │ PrimaryTeacher    │
// │ 2       │ Mr. Daniels   │ 101        │ Paraeducator      │
// │ 3       │ Dr. Patel     │ 101        │ Supervisor        │
// │ 3       │ Dr. Patel     │ 102        │ Supervisor        │
// │ 3       │ Dr. Patel     │ 103        │ Supervisor        │
// │ 3       │ Dr. Patel     │ 104        │ Supervisor        │
// └─────────┴───────────────┴────────────┴───────────────────┘
//
// Notice: Student 103 and 104 are NOT assigned to User 1 (Ms. Rivera) or
// User 2 (Mr. Daniels). If those users try to access those students, the
// authorization handler will get null from GetActiveAssignment and deny access.
//
// Dr. Patel (Supervisor) has Supervisor-type assignments to ALL students,
// which grants read-only access everywhere.
// =============================================================================

using RolesAssignments.Models;

namespace RolesAssignments.Data;

public class DummyAssignmentRepository : IAssignmentRepository
{
    // This list simulates the student_assignments table in the database.
    // Each entry represents one row — one relationship between a user and a student.
    private static readonly List<StudentAssignment> _assignments = new()
    {
        // Ms. Rivera (User 1) is the primary teacher for Students 101 and 102
        new StudentAssignment
        {
            Id = 1, UserId = 1, StudentId = 101,
            AssignmentType = AssignmentType.PrimaryTeacher,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },
        new StudentAssignment
        {
            Id = 2, UserId = 1, StudentId = 102,
            AssignmentType = AssignmentType.PrimaryTeacher,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },

        // Mr. Daniels (User 2) is a paraeducator assigned to Student 101
        new StudentAssignment
        {
            Id = 3, UserId = 2, StudentId = 101,
            AssignmentType = AssignmentType.Paraeducator,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },

        // Dr. Patel (User 3) is a supervisor for ALL students
        new StudentAssignment
        {
            Id = 4, UserId = 3, StudentId = 101,
            AssignmentType = AssignmentType.Supervisor,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },
        new StudentAssignment
        {
            Id = 5, UserId = 3, StudentId = 102,
            AssignmentType = AssignmentType.Supervisor,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },
        new StudentAssignment
        {
            Id = 6, UserId = 3, StudentId = 103,
            AssignmentType = AssignmentType.Supervisor,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },
        new StudentAssignment
        {
            Id = 7, UserId = 3, StudentId = 104,
            AssignmentType = AssignmentType.Supervisor,
            StartDate = new DateTime(2025, 9, 1), EndDate = null, IsActive = true
        },
    };

    /// <summary>
    /// Finds an active assignment between the given user and student.
    /// Returns null if no active assignment exists.
    /// </summary>
    public Task<StudentAssignment?> GetActiveAssignment(int userId, int studentId)
    {
        // This LINQ query does in-memory what the SQL query would do in a real database.
        var now = DateTime.UtcNow;

        var assignment = _assignments.FirstOrDefault(a =>
            a.UserId == userId
            && a.StudentId == studentId
            && a.IsActive
            && a.StartDate <= now
            && (a.EndDate == null || a.EndDate >= now));

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // In production, this method would use Dapper to query MySQL:
        //
        // public async Task<StudentAssignment?> GetActiveAssignment(int userId, int studentId)
        // {
        //     return await _db.QueryFirstOrDefaultAsync<StudentAssignment>(
        //         @"SELECT id, user_id AS UserId, student_id AS StudentId,
        //                  assignment_type AS AssignmentType,
        //                  start_date AS StartDate, end_date AS EndDate,
        //                  is_active AS IsActive
        //           FROM student_assignments
        //           WHERE user_id = @UserId
        //             AND student_id = @StudentId
        //             AND is_active = TRUE
        //             AND start_date <= CURDATE()
        //             AND (end_date IS NULL OR end_date >= CURDATE())
        //           LIMIT 1",
        //         new { UserId = userId, StudentId = studentId });
        // }

        return Task.FromResult(assignment);
    }
}
