// =============================================================================
// DummyStudentRepository.cs
// =============================================================================
// Hardcoded student data. The most interesting method here is
// GetAccessibleStudents, which demonstrates the "scoped list" pattern.
//
// THE IDEA:
// For single-resource endpoints (GET /students/101), the controller calls
// AuthorizeAsync to check access. But for LIST endpoints (GET /students),
// you can't load ALL students and then authorize each one — that's slow
// and leaks the existence of records the user shouldn't see.
//
// Instead, the list query itself is scoped through the assignments table.
// The query joins students through student_assignments, so it only returns
// students the user is assigned to. Unauthorized records never leave the
// database.
// =============================================================================

using RolesAssignments.Models;

namespace RolesAssignments.Data;

public class DummyStudentRepository : IStudentRepository
{
    // Simulates the students table
    private static readonly List<Student> _students = new()
    {
        new Student { Id = 101, Identifier = "Student A", ProgramYear = "2025-2026", Age = 8, IsDeleted = false },
        new Student { Id = 102, Identifier = "Student B", ProgramYear = "2025-2026", Age = 9, IsDeleted = false },
        new Student { Id = 103, Identifier = "Student C", ProgramYear = "2025-2026", Age = 7, IsDeleted = false },
        new Student { Id = 104, Identifier = "Student D", ProgramYear = "2025-2026", Age = 10, IsDeleted = false },
    };

    // We need the assignment data to scope the list query
    private readonly IAssignmentRepository _assignments;

    public DummyStudentRepository(IAssignmentRepository assignments)
    {
        _assignments = assignments;
    }

    /// <summary>
    /// Returns only the students that the given user is assigned to.
    /// In a real database, this would be a single SQL query with a JOIN
    /// through student_assignments. Here we simulate it in-memory.
    /// </summary>
    public Task<IEnumerable<StudentSummaryDto>> GetAccessibleStudents(int userId)
    {
        // In-memory simulation: we cast the dummy assignment repo to get
        // access to the underlying data. This is a prototype shortcut.
        // In production, this would be a SQL JOIN (see below).

        // We use the DummyAssignmentRepository to simulate the JOIN.
        // For each student, check if the user has an active assignment.
        var now = DateTime.UtcNow;
        var assignmentRepo = (DummyAssignmentRepository)_assignments;

        // Because we can't easily access the private static list from the
        // assignment repo in this simulation, we'll use GetActiveAssignment
        // for each student. This is an N+1 query in-memory — fine for a demo,
        // terrible in production (which is why the real version uses a JOIN).
        var results = new List<StudentSummaryDto>();
        foreach (var student in _students.Where(s => !s.IsDeleted))
        {
            var assignment = _assignments.GetActiveAssignment(userId, student.Id).Result;
            if (assignment is not null)
            {
                results.Add(new StudentSummaryDto
                {
                    Id = student.Id,
                    Identifier = student.Identifier,
                    ProgramYear = student.ProgramYear,
                    Age = student.Age,
                    AssignmentType = assignment.AssignmentType.ToString()
                });
            }
        }

        // ==========================================
        // REAL DATABASE VERSION (commented out):
        // ==========================================
        // In production, this is ONE query — no N+1 problem:
        //
        // return await _db.QueryAsync<StudentSummaryDto>(
        //     @"SELECT s.id AS Id,
        //              s.identifier AS Identifier,
        //              s.program_year AS ProgramYear,
        //              s.age AS Age,
        //              sa.assignment_type AS AssignmentType
        //       FROM students s
        //       INNER JOIN student_assignments sa ON sa.student_id = s.id
        //       WHERE sa.user_id = @UserId
        //         AND sa.is_active = TRUE
        //         AND sa.start_date <= CURDATE()
        //         AND (sa.end_date IS NULL OR sa.end_date >= CURDATE())
        //         AND s.is_deleted = FALSE
        //       ORDER BY s.identifier",
        //     new { UserId = userId });
        //
        // Notice: there is NO role check in the query. The assignments table
        // already encodes who can see what — supervisors have Supervisor-type
        // assignments to all students, so they naturally appear in the JOIN.

        return Task.FromResult<IEnumerable<StudentSummaryDto>>(results);
    }

    public Task<Student?> GetById(int studentId)
    {
        var student = _students.FirstOrDefault(s => s.Id == studentId && !s.IsDeleted);
        return Task.FromResult(student);
    }
}
