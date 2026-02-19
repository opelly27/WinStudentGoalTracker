// =============================================================================
// StudentSummaryDto.cs
// =============================================================================
// A "DTO" (Data Transfer Object) is a lightweight object designed specifically
// for sending data over the wire (in API responses). It contains only the
// fields the caller needs — no internal details, no navigation properties.
//
// This DTO is returned by the "Get My Students" list endpoint. It includes
// the student's basic info plus the assignment type, so the frontend knows
// what kind of access the current user has.
// =============================================================================

namespace RolesAssignments.Models;

public class StudentSummaryDto
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string ProgramYear { get; set; } = string.Empty;
    public int Age { get; set; }

    // This tells the frontend what the current user's relationship to this
    // student is. For supervisors who see all students, this might be null
    // for students they don't have a direct assignment to (though in this
    // prototype, supervisors always have Supervisor-type assignments).
    public string? AssignmentType { get; set; }
}
