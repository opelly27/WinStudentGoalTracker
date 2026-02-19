// =============================================================================
// ProgressEntryAuthorizationHandler.cs
// =============================================================================
// This is the SECOND LAYER of authorization. It handles the question:
// "Can this user edit/delete THIS SPECIFIC progress entry?"
//
// By the time this handler is called, the controller has ALREADY verified that
// the user has student-level access (using StudentAuthorizationHandler).
// This handler adds the extra ownership check.
//
// THE RULE:
// - Teachers (PrimaryTeacher / TemporaryCoverage) can edit or delete
//   ANY entry for their assigned students.
// - Paraeducators can ONLY edit or delete entries that THEY created.
//   (The CreatedByUserId on the entry must match their user ID.)
// - Supervisors cannot edit or delete anything. Since they don't match any
//   case below, they get an implicit deny.
//
// WHY TWO LAYERS?
// ---------------
// Imagine a paraeducator (Mr. Daniels) assigned to Student 101. He made an
// entry yesterday. The primary teacher (Ms. Rivera) also made one today.
//
// When Mr. Daniels tries to edit HIS entry:
//   Layer 1 (Student): "Is Mr. Daniels assigned to Student 101?" → Yes ✓
//   Layer 2 (Entry):   "Did Mr. Daniels create this entry?" → Yes ✓ → ALLOWED
//
// When Mr. Daniels tries to edit MS. RIVERA'S entry:
//   Layer 1 (Student): "Is Mr. Daniels assigned to Student 101?" → Yes ✓
//   Layer 2 (Entry):   "Did Mr. Daniels create this entry?" → No ✗ → DENIED
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RolesAssignments.Authorization.Resources;
using RolesAssignments.Data;
using RolesAssignments.Extensions;
using RolesAssignments.Models;

namespace RolesAssignments.Authorization.Handlers;

public class ProgressEntryAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, ProgressEntryResource>
//                                                             ^^^^^^^^^^^^^^^^^^^^^
//  Notice the second type parameter is ProgressEntryResource, not StudentResource.
//  This tells the framework: "Only call me when the resource is a ProgressEntryResource."
//  The StudentAuthorizationHandler will NOT fire for ProgressEntryResource, and this
//  handler will NOT fire for StudentResource. The framework routes automatically by type.
{
    // We need the assignment repository to check what type of assignment the
    // user has (PrimaryTeacher vs Paraeducator), because the rules differ.
    private readonly IAssignmentRepository _assignments;

    public ProgressEntryAuthorizationHandler(IAssignmentRepository assignments)
    {
        _assignments = assignments;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        ProgressEntryResource resource)
    {
        var userId = context.User.GetUserId();

        // Look up how this user is connected to the student this entry belongs to.
        var assignment = await _assignments.GetActiveAssignment(userId, resource.StudentId);

        // No assignment at all? No access to anything.
        if (assignment is null)
            return;

        switch (requirement.Name)
        {
            case nameof(Operations.EditProgressEntry):
            case nameof(Operations.DeleteProgressEntry):

                // Teachers can edit/delete ANY entry for their assigned students.
                // The student-level check (Layer 1) already confirmed they're assigned,
                // so here we just need to confirm their assignment type grants write access.
                if (assignment.AssignmentType is AssignmentType.PrimaryTeacher
                                              or AssignmentType.TemporaryCoverage)
                {
                    context.Succeed(requirement);
                }
                // Paraeducators can only touch entries THEY created.
                // We compare the entry's CreatedByUserId against the current user's ID.
                else if (assignment.AssignmentType == AssignmentType.Paraeducator
                         && resource.CreatedByUserId == userId)
                {
                    context.Succeed(requirement);
                }
                // Supervisors (and anyone else) fall through without Succeed() → DENIED.
                break;
        }
    }
}
