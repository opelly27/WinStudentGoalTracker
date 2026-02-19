// =============================================================================
// FakeAuthMiddleware.cs
// =============================================================================
// IN A REAL APPLICATION, this would be replaced by JWT bearer authentication,
// cookie authentication, or Identity. The user would log in once, receive a
// token or cookie, and every subsequent request would carry their identity.
//
// FOR THIS PROTOTYPE, we simulate login by reading an HTTP header:
//
//   X-User-Id: 1   →  You're Ms. Rivera (Teacher)
//   X-User-Id: 2   →  You're Mr. Daniels (Paraeducator)
//   X-User-Id: 3   →  You're Dr. Patel (Supervisor)
//
// This middleware runs BEFORE every request reaches a controller. It reads
// the header, looks up the user in a hardcoded list, and builds a
// ClaimsPrincipal — the same object that real authentication produces.
//
// This means the rest of the application (controllers, authorization handlers)
// works EXACTLY as it would with real authentication. The only fake part is
// HOW the user identity gets established.
//
// WHAT IS MIDDLEWARE?
// -------------------
// Middleware is code that runs in a pipeline for every HTTP request.
// Think of it like a series of checkpoints:
//   Request → [Middleware A] → [Middleware B] → [Controller] → Response
// Each middleware can inspect/modify the request, call the next one, or
// short-circuit the pipeline (e.g., return 401 without ever reaching the
// controller).
// =============================================================================

using System.Security.Claims;

namespace RolesAssignments.Middleware;

public class FakeAuthMiddleware
{
    // The next middleware in the pipeline. Middleware is like a chain —
    // each piece holds a reference to the next one and decides whether
    // to pass the request along.
    private readonly RequestDelegate _next;

    // Hardcoded user directory. In a real app, this would be a database lookup
    // during login, and the resulting claims would be stored in a JWT token.
    private static readonly Dictionary<int, (string Name, string Role)> _users = new()
    {
        { 1, ("Ms. Rivera",   "Teacher")       },
        { 2, ("Mr. Daniels",  "Paraeducator")  },
        { 3, ("Dr. Patel",    "Supervisor")     },
    };

    public FakeAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to read the X-User-Id header from the incoming request.
        // If it's missing or invalid, we let the request through without
        // setting up a user — the [Authorize] attribute on controllers
        // will reject it with a 401 Unauthorized.
        if (context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)
            && int.TryParse(userIdHeader, out var userId)
            && _users.TryGetValue(userId, out var user))
        {
            // Build a list of "claims" — key-value pairs that describe the user.
            // These are the same claims that would be embedded in a JWT token
            // or stored in an authentication cookie in a real system.
            var claims = new[]
            {
                // NameIdentifier = the user's unique ID (what GetUserId() reads)
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),

                // Name = the user's display name
                new Claim(ClaimTypes.Name, user.Name),

                // Role = what kind of user they are (what GetRole() reads)
                new Claim(ClaimTypes.Role, user.Role),
            };

            // Create a "ClaimsIdentity" (a collection of claims tied to an
            // authentication scheme). The second parameter "FakeAuth" is the
            // authentication scheme name — in a real app this would be
            // "Bearer" (for JWT) or "Cookies" (for cookie auth).
            var identity = new ClaimsIdentity(claims, "FakeAuth");

            // Wrap it in a ClaimsPrincipal and set it on the HTTP context.
            // From this point on, any code that reads HttpContext.User
            // (including controllers and authorization handlers) will see
            // this user as "logged in."
            context.User = new ClaimsPrincipal(identity);
        }

        // Pass the request to the next middleware in the pipeline.
        // If we didn't set up a user above, context.User will be an
        // anonymous/unauthenticated principal, and [Authorize] will block it.
        await _next(context);
    }
}
