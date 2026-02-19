// =============================================================================
// AssignmentType.cs
// =============================================================================
// This enum represents the RELATIONSHIP between a user and a student.
//
// Think of it this way: a user's ROLE (Teacher, Paraeducator, Supervisor) tells
// you WHAT they are. Their ASSIGNMENT TYPE tells you HOW they're connected to a
// specific student.
//
// For example, a Teacher might be the "PrimaryTeacher" for Student A, but have
// "TemporaryCoverage" for Student B (e.g., covering for a colleague on leave).
//
// This is stored in the `student_assignments` table in the database —
// each row says "User X is connected to Student Y as [this type]."
// =============================================================================

namespace RolesAssignments.Models;

public enum AssignmentType
{
    // The main teacher responsible for this student.
    // Has full read/write access to goals, entries, sensitive notes, etc.
    PrimaryTeacher,

    // A paraeducator (teaching assistant) assigned to help with this student.
    // Can view the student and add progress entries, but can only edit/delete
    // their OWN entries — not entries made by someone else.
    Paraeducator,

    // A supervisor (admin/principal) overseeing this student.
    // Has read-only access: can view everything and generate reports,
    // but cannot create, edit, or delete anything.
    Supervisor,

    // A teacher temporarily covering for the primary teacher (e.g., sick leave).
    // In this prototype, treated similarly to PrimaryTeacher for simplicity,
    // but in a real system you might limit what they can do.
    TemporaryCoverage
}
