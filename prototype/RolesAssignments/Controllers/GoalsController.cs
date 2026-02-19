// =============================================================================
// GoalsController.cs
// =============================================================================
// Full CRUD for goals, nested under /api/students/{studentId}/goals.
//
// Every action follows the same pattern:
//   1. AUTHORIZE — ask the authorization system "can this user do this?"
//   2. LOAD     — fetch the resource from the data layer
//   3. ACT      — perform the business logic
//
// Notice that the controller has ZERO role-checking logic. There's no
// `if (user.Role == "Teacher")` anywhere. All of that lives in the
// StudentAuthorizationHandler. If the rules change (e.g., paraeducators
// should also be able to create goals), you update the handler — not the
// controller.
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
[Route("api/students/{studentId}/goals")]
[Authorize]
public class GoalsController : ControllerBase
{
    private readonly IAuthorizationService _auth;
    private readonly IGoalRepository _goals;

    public GoalsController(IAuthorizationService auth, IGoalRepository goals)
    {
        _auth = auth;
        _goals = goals;
    }

    /// <summary>
    /// GET /api/students/{studentId}/goals
    /// Lists all goals for a student. Requires ViewStudent permission.
    ///
    /// Any assigned user (teacher, para, supervisor) can view goals.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetGoalsForStudent(int studentId)
    {
        // Step 1: AUTHORIZE — can this user view this student?
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.ViewStudent);
        if (!authResult.Succeeded) return Forbid();

        // Step 2 & 3: LOAD and return
        var goals = await _goals.GetByStudentId(studentId);
        return Ok(goals);
    }

    /// <summary>
    /// GET /api/students/{studentId}/goals/{goalId}
    /// Returns a single goal. Requires ViewStudent permission.
    /// </summary>
    [HttpGet("{goalId}")]
    public async Task<IActionResult> GetGoal(int studentId, int goalId)
    {
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.ViewStudent);
        if (!authResult.Succeeded) return Forbid();

        var goal = await _goals.GetById(goalId);

        // Double-check that the goal actually belongs to this student.
        // This prevents a URL manipulation attack where someone guesses a
        // goal ID that belongs to a different student.
        if (goal is null || goal.StudentId != studentId) return NotFound();

        return Ok(goal);
    }

    /// <summary>
    /// POST /api/students/{studentId}/goals
    /// Creates a new goal. Requires CreateGoal permission.
    ///
    /// Only PrimaryTeacher and TemporaryCoverage assignments grant this.
    /// Paraeducators and Supervisors will get 403 Forbidden.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateGoal(int studentId, [FromBody] CreateGoalRequest request)
    {
        // Note: We check CreateGoal here, not ViewStudent. The handler grants
        // CreateGoal only to PrimaryTeacher and TemporaryCoverage.
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.CreateGoal);
        if (!authResult.Succeeded) return Forbid();

        var goalId = await _goals.Create(studentId, request, User.GetUserId());

        // Return 201 Created with a Location header pointing to the new resource.
        // CreatedAtAction generates the URL automatically based on the named action.
        return CreatedAtAction(nameof(GetGoal), new { studentId, goalId }, null);
    }

    /// <summary>
    /// PUT /api/students/{studentId}/goals/{goalId}
    /// Updates an existing goal. Requires EditGoal permission.
    ///
    /// Only PrimaryTeacher and TemporaryCoverage assignments grant this.
    /// </summary>
    [HttpPut("{goalId}")]
    public async Task<IActionResult> UpdateGoal(
        int studentId, int goalId, [FromBody] UpdateGoalRequest request)
    {
        var authResult = await _auth.AuthorizeAsync(
            User, new StudentResource(studentId), Operations.EditGoal);
        if (!authResult.Succeeded) return Forbid();

        var goal = await _goals.GetById(goalId);
        if (goal is null || goal.StudentId != studentId) return NotFound();

        await _goals.Update(goalId, request, User.GetUserId());

        // 204 No Content — the update succeeded but there's nothing to return.
        return NoContent();
    }
}
