// =============================================================================
// ProgressEntry.cs
// =============================================================================
// Represents a single log entry tracking a student's progress toward a goal.
// For example: "Feb 19 — Student read 15 words per minute today."
//
// This is the entity that demonstrates the TWO-LAYER authorization check:
//
//   Layer 1 (Student-level): "Is this user assigned to the student at all?"
//   Layer 2 (Entry-level):   "Does this user own THIS specific entry?"
//
// Teachers can edit/delete ANY entry for their assigned students.
// Paraeducators can only edit/delete entries THEY created.
// Supervisors cannot edit/delete anything.
//
// The key field for Layer 2 is `CreatedByUserId` — it tells us who wrote
// this entry, which determines ownership.
// =============================================================================

namespace RolesAssignments.Models;

public class ProgressEntry
{
    public int Id { get; set; }

    // Which student this entry is about
    public int StudentId { get; set; }

    // The content of the progress note
    public string Notes { get; set; } = string.Empty;

    // WHO created this entry — this is the field used for ownership checks.
    // When a paraeducator tries to edit an entry, we compare this field
    // against the current user's ID to decide if they're allowed.
    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Soft-delete: mark as deleted instead of actually removing the row
    public bool IsDeleted { get; set; }
}
