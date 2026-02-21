using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WinStudentGoalTracker.BaseClasses;

public class BaseController : ControllerBase
{
    protected (Guid userId, ActionResult? error) GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return (Guid.Empty, Unauthorized("Missing or invalid user_id claim."));
        }

        return (userId, null);
    }

    protected (string email, List<string> roles, ActionResult? error) GetUserDetailsFromClaims()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return (string.Empty, new List<string>(), Unauthorized("Missing email claim."));
        }

        var roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();
        return (email, roles, null);
    }

    protected bool HasRole(string role)
    {
        return User.IsInRole(role);
    }

    protected bool HasAnyRole(params string[] roles)
    {
        return roles.Any(User.IsInRole);
    }
}
