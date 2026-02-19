// =============================================================================
// ClaimsPrincipalExtensions.cs
// =============================================================================
// WHAT IS AN EXTENSION METHOD?
// ----------------------------
// Normally, to add a method to a class, you'd edit that class. But you can't
// edit ClaimsPrincipal — it's a built-in .NET class owned by Microsoft.
//
// C# has a feature called "extension methods" that lets you write a static
// method in a static class, but call it AS IF it were an instance method on
// another class. The trick is the `this` keyword before the first parameter:
//
//   public static int GetUserId(this ClaimsPrincipal principal)
//                               ^^^^
//                               This makes it an extension method.
//
// Now you can write:
//   context.User.GetUserId()
//
// Instead of:
//   ClaimsPrincipalExtensions.GetUserId(context.User)
//
// Both forms compile to the exact same code. The first is just nicer to read.
// The one requirement is that the namespace containing the extension class must
// be imported with `using` wherever you want to use it.
//
// WHAT IS A ClaimsPrincipal?
// --------------------------
// When a user logs in, their identity is represented as a ClaimsPrincipal.
// It contains "claims" — key-value pairs like:
//   - NameIdentifier = "1"    (the user's ID)
//   - Role           = "Teacher"
//   - Email          = "rivera@school.edu"
//
// These claims are typically set during login (packed into a JWT token or
// cookie) and are available on every subsequent request via HttpContext.User.
//
// The extension methods below make it easy to extract specific values without
// writing the same verbose claim-lookup code in every controller and handler.
// =============================================================================

using System.Security.Claims;

namespace RolesAssignments.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the user's numeric ID from their claims.
    /// The claim is stored under ClaimTypes.NameIdentifier, which is a
    /// standard claim type meaning "the unique identifier for this user."
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        // FindFirstValue searches through the claims for the first one
        // with the given type, and returns its string value.
        var claim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (claim is null)
            throw new InvalidOperationException(
                "User ID claim not found. This means the authentication " +
                "middleware didn't set up the user's claims properly.");

        // Claims are always strings, so we parse to int.
        return int.Parse(claim);
    }

    /// <summary>
    /// Extracts the user's role from their claims (e.g., "Teacher").
    /// </summary>
    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Role)
            ?? throw new InvalidOperationException(
                "Role claim not found. This means the authentication " +
                "middleware didn't set up the user's claims properly.");
    }
}
