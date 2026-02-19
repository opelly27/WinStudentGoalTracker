// =============================================================================
// Goal.cs
// =============================================================================
// Represents a learning goal that belongs to a specific student.
// For example: "Student will read 20 words per minute by end of semester."
//
// Only a PrimaryTeacher (or TemporaryCoverage) can create or edit goals.
// Everyone else assigned to the student can VIEW them, but not change them.
// =============================================================================

namespace RolesAssignments.Models;

public class Goal
{
    public int Id { get; set; }

    // The student this goal belongs to.
    // This is the foreign key back to the students table.
    public int StudentId { get; set; }

    // The text of the goal itself
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Who created this goal and when
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
