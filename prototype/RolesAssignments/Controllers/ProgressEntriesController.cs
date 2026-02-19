// =============================================================================
// ProgressEntriesController.cs
// =============================================================================
// This controller demonstrates the TWO-LAYER authorization pattern.
//
// For VIEWING and CREATING entries, we only need Layer 1 (student-level):
//   "Is this user assigned to the student?"
//
// For EDITING and DELETING entries, we need BOTH layers:
//   Layer 1 (student-level): "Is this user assigned to the student?"
//   Layer 2 (entry-level):   "Can this user edit THIS specific entry?"
//
// The entry-level check is needed because paraeducators can only edit/delete
// entries they created themselves. Teachers can edit/delete any entry.
//
// THE FLOW for edit/delete:
//   1. Check student-level access (StudentResource + ViewStudent)
//   2. Load the entry from the database
//   3. Check entry-level ownership (ProgressEntryResource + EditProgressEntry)
//   4. If both pass, proceed with the operation
//
// If either check fails, the user gets 403 Forbidden.
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RolesAssignments.Authorization;
using RolesAssignments.Authorization.Resources;
using RolesAssignments.Data;
using RolesAssignments.Extensions;
using RolesAssignments.Models;

namespace RolesAssignments.Controllers;

[ApiController]
[Route("api/students/{studentId}/entries")]
[Authorize]
public class ProgressEntriesController : ControllerBase
{
    private readonly IAuthorizationService _auth;
    private readonly IProgressEntryRepository _entries;

    public ProgressEntriesController(IAuthorizationService auth, IProgressEntryRepository entries)
    {
        _auth = auth;
        _entries = entries;
    }

    /// <summary>
    /// GET /api/students/{studentId}/entries
    /// Lists all progress entries for a student. Requires ViewStudent permission.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetEntriesForStudent(int studentId)
    {
        // Just a student-level check — anyone assigned can view entries.
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.ViewStudent);
        if (!authResult.Succeeded) return Forbid();

        var entries = await _entries.GetByStudentId(studentId);
        return Ok(entries);
    }

    /// <summary>
    /// GET /api/students/{studentId}/entries/{entryId}
    /// Returns a single progress entry. Requires ViewStudent permission.
    /// </summary>
    [HttpGet("{entryId}")]
    public async Task<IActionResult> GetEntry(int studentId, int entryId)
    {
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.ViewStudent);
        if (!authResult.Succeeded) return Forbid();

        var entry = await _entries.GetById(entryId);
        if (entry is null || entry.StudentId != studentId) return NotFound();

        return Ok(entry);
    }

    /// <summary>
    /// POST /api/students/{studentId}/entries
    /// Creates a new progress entry. Requires AddProgressEntry permission.
    ///
    /// Teachers and Paraeducators can both add entries.
    /// Supervisors cannot.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddEntry(int studentId, [FromBody] CreateEntryRequest request)
    {
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.AddProgressEntry);
        if (!authResult.Succeeded) return Forbid();

        var entryId = await _entries.Create(studentId, request, User.GetUserId());
        return CreatedAtAction(nameof(GetEntry), new { studentId, entryId }, null);
    }

    /// <summary>
    /// PUT /api/students/{studentId}/entries/{entryId}
    /// Updates an existing progress entry.
    ///
    /// THIS IS WHERE THE TWO-LAYER CHECK HAPPENS:
    ///
    /// Layer 1: Can the user access this student at all?
    ///   → Uses StudentResource + ViewStudent
    ///   → Checked by StudentAuthorizationHandler
    ///
    /// Layer 2: Can the user edit THIS SPECIFIC entry?
    ///   → Uses ProgressEntryResource + EditProgressEntry
    ///   → Checked by ProgressEntryAuthorizationHandler
    ///   → Teachers: can edit any entry
    ///   → Paraeducators: can only edit entries they created
    ///   → Supervisors: cannot edit anything
    /// </summary>
    [HttpPut("{entryId}")]
    public async Task<IActionResult> UpdateEntry(
        int studentId, int entryId, [FromBody] UpdateEntryRequest request)
    {
        // LAYER 1: Student-level access check
        var studentAuth = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.ViewStudent);
        if (!studentAuth.Succeeded) return Forbid();

        // Load the entry — we need it to check ownership in Layer 2
        var entry = await _entries.GetById(entryId);
        if (entry is null || entry.StudentId != studentId) return NotFound();

        // LAYER 2: Entry-level ownership check
        // Notice we're using a DIFFERENT resource type (ProgressEntryResource)
        // and a DIFFERENT operation (EditProgressEntry).
        // This causes the framework to call ProgressEntryAuthorizationHandler
        // instead of StudentAuthorizationHandler.
        var entryAuth = await _auth.AuthorizeAsync(
            User,
            new ProgressEntryResource(entry.StudentId, entry.Id, entry.CreatedByUserId),
            Operations.EditProgressEntry);
        if (!entryAuth.Succeeded) return Forbid();

        // Both layers passed — proceed with the update
        await _entries.Update(entryId, request, User.GetUserId());
        return NoContent();
    }

    /// <summary>
    /// DELETE /api/students/{studentId}/entries/{entryId}
    /// Soft-deletes a progress entry. Same two-layer check as UpdateEntry.
    ///
    /// Teachers: can delete any entry for their assigned students.
    /// Paraeducators: can only delete entries they created.
    /// Supervisors: cannot delete.
    /// </summary>
    [HttpDelete("{entryId}")]
    public async Task<IActionResult> DeleteEntry(int studentId, int entryId)
    {
        // Layer 1: Student access
        var studentAuth = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.ViewStudent);
        if (!studentAuth.Succeeded) return Forbid();

        // Load the entry
        var entry = await _entries.GetById(entryId);
        if (entry is null || entry.StudentId != studentId) return NotFound();

        // Layer 2: Entry ownership
        var entryAuth = await _auth.AuthorizeAsync(
            User,
            new ProgressEntryResource(entry.StudentId, entry.Id, entry.CreatedByUserId),
            Operations.DeleteProgressEntry);
        if (!entryAuth.Succeeded) return Forbid();

        // Soft-delete (marks as deleted, doesn't remove the row)
        await _entries.SoftDelete(entryId, User.GetUserId());
        return Ok();
    }
}
