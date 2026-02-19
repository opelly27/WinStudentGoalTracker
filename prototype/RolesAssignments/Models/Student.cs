// =============================================================================
// Student.cs
// =============================================================================
// A simple student entity. In a real app this would map to the `students` table.
//
// Note: students don't "belong" to a teacher directly. Instead, the connection
// is made through the `student_assignments` table (see StudentAssignment.cs).
// This keeps the data model flexible — a student can have multiple adults
// working with them, each with a different level of access.
// =============================================================================

namespace RolesAssignments.Models;

public class Student
{
    public int Id { get; set; }

    // A human-readable label like "Student A" or an internal identifier.
    // In a real system this might be a school-issued ID number.
    public string Identifier { get; set; } = string.Empty;

    // e.g., "2025-2026"
    public string ProgramYear { get; set; } = string.Empty;

    public int Age { get; set; }

    // Soft-delete flag. Instead of removing rows from the database,
    // we mark them as deleted so we can still reference historical data.
    public bool IsDeleted { get; set; }
}
