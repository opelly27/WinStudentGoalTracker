// =============================================================================
// StudentResource.cs
// =============================================================================
// This is a "resource record" — a tiny data carrier that gets passed to the
// authorization system so it knows WHICH student we're asking about.
//
// When you call:
//   await _auth.AuthorizeAsync(User, new StudentResource(studentId), Operations.EditGoal);
//
// ...the framework takes this object and hands it to every authorization
// handler that declared it handles StudentResource. The handler then uses
// the StudentId to look up the user's assignment and make a decision.
//
// A "record" in C# is a special class that's designed to just hold data.
// Writing `public record StudentResource(int StudentId);` is shorthand for
// a class with a constructor, a read-only property, equality checks, etc.
// It's the modern C# way of saying "this is just a bag of values."
// =============================================================================

namespace RolesAssignments.Authorization.Resources;

public record StudentResource(int StudentId);
