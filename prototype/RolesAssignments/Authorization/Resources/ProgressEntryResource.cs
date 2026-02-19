// =============================================================================
// ProgressEntryResource.cs
// =============================================================================
// This resource record carries enough information for the authorization handler
// to decide if the current user can edit or delete a SPECIFIC progress entry.
//
// It includes:
//   - StudentId: needed to verify the user has any access to this student at all
//   - EntryId: identifies which entry we're talking about
//   - CreatedByUserId: WHO originally wrote this entry — this is the key field
//     for the "you can only edit your own entries" rule.
//
// The reason we pass CreatedByUserId here (instead of making the handler look
// it up) is that the controller already loaded the entry to check if it exists.
// Since we already have the data, we pass it along to avoid a redundant
// database call inside the handler.
// =============================================================================

namespace RolesAssignments.Authorization.Resources;

public record ProgressEntryResource(int StudentId, int EntryId, int CreatedByUserId);
