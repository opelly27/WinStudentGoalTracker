// =============================================================================
// Requests.cs
// =============================================================================
// These are the "request" DTOs — the shapes of data that the API expects to
// RECEIVE from the caller when creating or updating resources.
//
// They're intentionally simple: just the fields the caller controls.
// Things like Id, CreatedByUserId, and CreatedAt are set by the server,
// not by the caller.
//
// In a real app, you'd add validation attributes here (e.g., [Required],
// [MaxLength(500)]) to enforce rules before the data reaches your logic.
// =============================================================================

namespace RolesAssignments.Models;

// --- Goal Requests ---

public class CreateGoalRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateGoalRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// --- Progress Entry Requests ---

public class CreateEntryRequest
{
    public string Notes { get; set; } = string.Empty;
}

public class UpdateEntryRequest
{
    public string Notes { get; set; } = string.Empty;
}
