// =============================================================================
// Operations.cs
// =============================================================================
// This class defines every OPERATION (action) that exists in the system.
// Think of it as a menu of "things a user might try to do."
//
// Each field is an OperationAuthorizationRequirement — a built-in ASP.NET Core
// class that just wraps a string name. The authorization handlers switch on
// this name to decide if the user is allowed to perform the action.
//
// WHY nameof()?
// -------------
// You'll see `nameof(ViewStudent)` everywhere. This is a C# feature that takes
// the NAME of the thing you point it at and turns it into a string.
//
// So `nameof(ViewStudent)` produces the string "ViewStudent".
//
// It's the same as writing `Name = "ViewStudent"`, but safer — if you rename
// the field later, nameof() updates automatically. With a hardcoded string,
// you'd have a silent bug where the name no longer matches.
//
// These are NOT connected to controller methods or endpoint names.
// They're a self-contained vocabulary of actions that the handlers interpret.
// =============================================================================

using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace RolesAssignments.Authorization;

public static class Operations
{
    // --- Student-level operations ---

    // Can the user SEE this student's information at all?
    public static readonly OperationAuthorizationRequirement ViewStudent =
        new() { Name = nameof(ViewStudent) };

    // Can the user create a new goal for this student?
    public static readonly OperationAuthorizationRequirement CreateGoal =
        new() { Name = nameof(CreateGoal) };

    // Can the user edit an existing goal for this student?
    public static readonly OperationAuthorizationRequirement EditGoal =
        new() { Name = nameof(EditGoal) };

    // Can the user add a new progress entry for this student?
    public static readonly OperationAuthorizationRequirement AddProgressEntry =
        new() { Name = nameof(AddProgressEntry) };

    // --- Entry-level operations (checked AFTER the student-level check) ---

    // Can the user edit this specific progress entry?
    // (For teachers: yes, any entry. For paraeducators: only their own.)
    public static readonly OperationAuthorizationRequirement EditProgressEntry =
        new() { Name = nameof(EditProgressEntry) };

    // Can the user delete this specific progress entry?
    public static readonly OperationAuthorizationRequirement DeleteProgressEntry =
        new() { Name = nameof(DeleteProgressEntry) };

    // --- Other operations ---

    // Can the user view sensitive/confidential notes about this student?
    public static readonly OperationAuthorizationRequirement ViewSensitiveNotes =
        new() { Name = nameof(ViewSensitiveNotes) };

    // Can the user generate reports for this student?
    public static readonly OperationAuthorizationRequirement GenerateReport =
        new() { Name = nameof(GenerateReport) };
}
