// =============================================================================
// IAssignmentRepository.cs
// =============================================================================
// This interface defines WHAT the assignment repository can do, without saying
// HOW it does it. In this prototype, the "how" is hardcoded dummy data.
// In production, the "how" would be a MySQL query via Dapper.
//
// WHY USE AN INTERFACE?
// ---------------------
// By coding against IAssignmentRepository (the interface) instead of a
// concrete class, you can swap implementations without changing any other code.
// Today: DummyAssignmentRepository (hardcoded). Tomorrow: DapperAssignmentRepository
// (real database). The controllers, handlers, and everything else don't change —
// they only know about the interface, not the implementation behind it.
//
// The swap happens in one place: Program.cs, where you register the service.
// =============================================================================

using RolesAssignments.Models;

namespace RolesAssignments.Data;

public interface IAssignmentRepository
{
    /// <summary>
    /// Looks up the active assignment between a specific user and a specific student.
    /// Returns null if no active assignment exists (meaning the user has no access
    /// to that student).
    ///
    /// "Active" means:
    ///   - is_active is true
    ///   - start_date is today or earlier
    ///   - end_date is null (ongoing) or today or later
    /// </summary>
    Task<StudentAssignment?> GetActiveAssignment(int userId, int studentId);
}
