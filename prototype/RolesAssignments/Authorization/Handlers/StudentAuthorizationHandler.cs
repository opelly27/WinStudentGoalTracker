// =============================================================================
// StudentAuthorizationHandler.cs
// =============================================================================
// THIS IS THE MOST IMPORTANT FILE IN THE AUTHORIZATION SYSTEM.
//
// This handler answers the question: "Can this user perform [operation] on
// this student?" It's called automatically by ASP.NET Core whenever you call:
//
//   await _auth.AuthorizeAsync(User, new StudentResource(studentId), Operations.SomeOp);
//
// HOW IT GETS CALLED (the chain):
// --------------------------------
// 1. Your controller calls AuthorizeAsync(user, resource, operation).
// 2. The framework looks through all registered IAuthorizationHandler classes.
// 3. This class inherits from AuthorizationHandler<OperationAuthorizationRequirement, StudentResource>.
//    The two generic type parameters tell the framework: "I handle checks where
//    the requirement is an OperationAuthorizationRequirement AND the resource
//    is a StudentResource."
// 4. The framework's base class checks: does the requirement type match? Does
//    the resource type match? If yes to both, it calls HandleRequirementAsync.
// 5. If you call context.Succeed(requirement), the check passes. If you don't
//    call Succeed, the check fails (implicit deny — secure by default).
//
// WHAT IT DECIDES:
// ----------------
// - Supervisors: can view students and generate reports. Cannot write anything.
// - Anyone with an active assignment: can view the student.
// - PrimaryTeacher (or TemporaryCoverage): can create/edit goals and view notes.
// - Teacher & Paraeducator: can add progress entries.
// - No assignment at all: denied everything (the method returns without calling Succeed).
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RolesAssignments.Authorization.Resources;
using RolesAssignments.Data;
using RolesAssignments.Extensions;
using RolesAssignments.Models;

namespace RolesAssignments.Authorization.Handlers;

public class StudentAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, StudentResource>
{
    // We inject the assignment repository so we can look up whether the user
    // is assigned to the student. In this prototype, the repository returns
    // hardcoded data. In production, it would query MySQL via Dapper.
    private readonly IAssignmentRepository _assignments;

    public StudentAuthorizationHandler(IAssignmentRepository assignments)
    {
        _assignments = assignments;
    }

    // This is the method the framework calls. We NEVER call it directly ourselves.
    //
    // Parameters (all provided by the framework):
    //   context     - Carries the ClaimsPrincipal (the logged-in user). Also the
    //                 object you call .Succeed() on to approve the request.
    //   requirement - The operation being attempted (e.g., Operations.EditGoal).
    //                 Its .Name property is the string we switch on.
    //   resource    - The StudentResource we passed in from the controller.
    //                 Contains the StudentId we're checking access for.
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        StudentResource resource)
    {
        // Extract the current user's ID from their claims.
        // (See ClaimsPrincipalExtensions.cs for how this extension method works.)
        var userId = context.User.GetUserId();

        // Look up the user's assignment to this specific student.
        // If this returns null, the user has NO connection to this student at all.
        var assignment = await _assignments.GetActiveAssignment(userId, resource.StudentId);

        // NO ASSIGNMENT = NO ACCESS
        // If the user isn't assigned to this student, we simply return without
        // calling Succeed(). This is an implicit denial — the framework treats
        // "nobody said yes" as "access denied."
        if (assignment is null)
            return;

        // The user IS assigned. Now decide based on what they're trying to do
        // and what type of assignment they have.
        switch (requirement.Name)
        {
            // --- VIEWING ---
            // Any assigned user can view the student, regardless of assignment type.
            // This covers teachers, paraeducators, supervisors, and temp coverage.
            case nameof(Operations.ViewStudent):
                context.Succeed(requirement);
                break;

            // --- REPORTS ---
            // Reports are read-only, so any assigned user can generate them.
            case nameof(Operations.GenerateReport):
                context.Succeed(requirement);
                break;

            // --- CREATING AND EDITING GOALS ---
            // Only the PrimaryTeacher (or someone with TemporaryCoverage) can
            // create or modify goals. Paraeducators can see goals but not change
            // them. Supervisors can view but not write.
            case nameof(Operations.CreateGoal):
            case nameof(Operations.EditGoal):
                if (assignment.AssignmentType is AssignmentType.PrimaryTeacher
                                              or AssignmentType.TemporaryCoverage)
                {
                    context.Succeed(requirement);
                }
                break;

            // --- ADDING PROGRESS ENTRIES ---
            // Both teachers and paraeducators can add progress entries.
            // (The edit/delete rules are more restrictive — see
            // ProgressEntryAuthorizationHandler.cs for those.)
            // Supervisors cannot add entries.
            case nameof(Operations.AddProgressEntry):
                if (assignment.AssignmentType is AssignmentType.PrimaryTeacher
                                              or AssignmentType.TemporaryCoverage
                                              or AssignmentType.Paraeducator)
                {
                    context.Succeed(requirement);
                }
                break;

            // --- VIEWING SENSITIVE NOTES ---
            // Only the primary teacher can see confidential notes about a student.
            case nameof(Operations.ViewSensitiveNotes):
                if (assignment.AssignmentType == AssignmentType.PrimaryTeacher)
                {
                    context.Succeed(requirement);
                }
                break;

            // If the operation doesn't match any of the above cases, we fall
            // through without calling Succeed() — which means ACCESS DENIED.
            // This is "deny by default" and is the safest approach.
        }
    }
}
