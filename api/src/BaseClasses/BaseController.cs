using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WinStudentGoalTracker.BaseClasses;

public class BaseController : ControllerBase
{
    protected (Guid userId, string email, Guid programId, string role, ActionResult? error) GetProgramUserFromClaims()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return (Guid.Empty, string.Empty, Guid.Empty, string.Empty, Unauthorized("Missing or invalid user_id claim."));

        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
            return (Guid.Empty, string.Empty, Guid.Empty, string.Empty, Unauthorized("Missing email claim."));

        var programIdClaim = User.FindFirst("program_id")?.Value;
        if (!Guid.TryParse(programIdClaim, out var programId))
            return (Guid.Empty, string.Empty, Guid.Empty, string.Empty, Unauthorized("Missing or invalid program_id claim."));

        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrWhiteSpace(role))
            return (Guid.Empty, string.Empty, Guid.Empty, string.Empty, Unauthorized("Missing role claim."));

        return (userId, email, programId, role, null);
    }
}
