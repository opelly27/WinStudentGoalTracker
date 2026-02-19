// =============================================================================
// StudentsController.cs
// =============================================================================
// This controller demonstrates TWO different authorization patterns:
//
// 1. LIST endpoint (GET /api/students):
//    Authorization is handled IN THE QUERY — the repository only returns
//    students the user is assigned to. No AuthorizeAsync call needed because
//    unauthorized records never leave the database.
//
// 2. SINGLE-RESOURCE endpoint (GET /api/students/{id}):
//    Authorization is checked EXPLICITLY by calling AuthorizeAsync with
//    a StudentResource. The authorization handler decides yes/no.
//
// WHY THE DIFFERENCE?
// -------------------
// For a single resource, you can load it and then ask "can this user see this?"
// For a list, you can't load ALL records and filter — that's slow and leaks
// information (the user could infer how many total records exist). So the
// query itself is scoped to only return what the user is allowed to see.
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RolesAssignments.Authorization;
using RolesAssignments.Authorization.Resources;
using RolesAssignments.Data;
using RolesAssignments.Extensions;

namespace RolesAssignments.Controllers;

[ApiController]
[Route("api/students")]
[Authorize]  // This attribute means: "the user must be authenticated (logged in)
             // to access ANY endpoint in this controller." It doesn't check roles
             // or permissions — just that the user is someone, not anonymous.
public class StudentsController : ControllerBase
{
    private readonly IAuthorizationService _auth;
    private readonly IStudentRepository _students;

    // These dependencies are injected by the DI container (configured in Program.cs).
    // The controller doesn't know or care whether it's getting dummy data or real data.
    public StudentsController(IAuthorizationService auth, IStudentRepository students)
    {
        _auth = auth;
        _students = students;
    }

    /// <summary>
    /// GET /api/students
    /// Returns the list of students that the current user is allowed to see.
    ///
    /// This is the "scoped list" pattern:
    /// - Ms. Rivera (User 1) → sees Students 101, 102 (her assigned students)
    /// - Mr. Daniels (User 2) → sees Student 101 only (his assignment)
    /// - Dr. Patel (User 3) → sees all students (supervisor assignments)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyStudents()
    {
        // Get the current user's ID from their claims.
        var userId = User.GetUserId();

        // The repository query is already scoped — it joins through the
        // assignments table, so it only returns students this user has access to.
        // No AuthorizeAsync call needed here.
        var students = await _students.GetAccessibleStudents(userId);

        return Ok(students);
    }

    /// <summary>
    /// GET /api/students/{studentId}
    /// Returns a single student. Requires the user to be assigned to this student.
    ///
    /// This is the "authorize-then-load" pattern:
    /// 1. Check: can this user view this student?
    /// 2. If yes, load and return the data.
    /// 3. If no, return 403 Forbidden.
    /// </summary>
    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetStudent(int studentId)
    {
        // AUTHORIZATION CHECK
        // We create a StudentResource (just a carrier with the student ID)
        // and pass it to AuthorizeAsync along with the operation we want to
        // perform (ViewStudent). The framework then calls our
        // StudentAuthorizationHandler, which checks the user's assignment.
        var authResult = await _auth.AuthorizeAsync(
            User,                                // The current user (ClaimsPrincipal)
            new StudentResource(studentId),       // The resource we're asking about
            Operations.ViewStudent);              // The operation we want to perform

        // If the handler didn't call context.Succeed(), the result is "not succeeded"
        // and we return 403 Forbidden. The user is authenticated (we know who they
        // are) but not authorized (they're not allowed to do this).
        if (!authResult.Succeeded)
            return Forbid();

        // Authorization passed — now load the actual data.
        var student = await _students.GetById(studentId);
        if (student is null)
            return NotFound();

        return Ok(student);
    }
}
