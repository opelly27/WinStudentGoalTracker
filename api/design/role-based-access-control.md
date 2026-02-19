# Role-Based Access Control Design Document

## Student Goal & Progress Tracking Application

---

## 1. Overview

This document defines the authorization model for the Student Goal & Progress Tracking application. The system uses a combination of role-based access control (RBAC) and resource-level assignment enforcement to determine what each user can do and to whom.

All authorization decisions flow from two sources of truth:

- **User role** — defines the category of actions a user may perform (e.g., Teacher, Paraeducator, Supervisor).
- **Student assignments** — defines which students a user has access to and whether they hold primary responsibility.

These two dimensions are evaluated together at every access point. A user's role determines what operations are available to them in general, and their assignment to a specific student determines whether they can perform those operations against that student's records.

---

## 2. Roles

### 2.1 Teacher

Teachers are the primary instructional staff responsible for student documentation.

- May be assigned to multiple students.
- May hold primary assignment for one or more students.
- Primary assignment grants full control over the student's goals and records.
- Non-primary assignment grants read access and the ability to add progress entries.

### 2.2 Paraeducator

Paraeducators are support staff who assist students in the field.

- May be assigned to multiple students.
- May add progress entries and critical notes for assigned students.
- May edit or delete only entries they personally created.
- Cannot create, edit, or archive goals.
- Cannot view sensitive records unless explicitly permitted.

### 2.3 Supervisor

Supervisors are oversight users who review documentation for evaluation, audit, or legal purposes.

- Have read-only access to all student records, entries, and reports.
- Cannot create, modify, or delete any records.
- Access is modeled through student assignments, ensuring uniform query behavior.

---

## 3. Student Assignments

### 3.1 Design Principle

The `student_assignments` table is the single source of truth for access control. Every user — regardless of role — must have an active assignment to a student in order to access that student's records. This eliminates role-based branching in queries and ensures that all access is explicit and auditable.

### 3.2 Assignment Schema

```sql
CREATE TABLE student_assignments (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    student_id INT NOT NULL,
    is_primary BOOLEAN NOT NULL DEFAULT FALSE,
    start_date DATE NOT NULL,
    end_date DATE NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (student_id) REFERENCES students(id)
);
```

### 3.3 Field Definitions

| Field | Description |
|---|---|
| `user_id` | The user being granted access. |
| `student_id` | The student the user is being assigned to. |
| `is_primary` | Whether this user holds primary responsibility for this student. Primary users may create and manage goals, edit the student profile, and view sensitive records. |
| `start_date` | The date the assignment becomes effective. |
| `end_date` | The date the assignment expires. NULL indicates an open-ended assignment. |
| `is_active` | Whether the assignment is currently active. Supports manual deactivation independent of date range. |
| `created_at` | Timestamp of assignment creation. |
| `created_by` | The user who created the assignment. |

### 3.4 Primary Assignment Rules

- Only users with the Teacher role may hold a primary assignment (`is_primary = TRUE`).
- A student should have exactly one primary assignment at any given time.
- Paraeducators and Supervisors always have `is_primary = FALSE`.
- The `is_primary` flag determines access to privileged operations such as goal management and student profile editing.

### 3.5 Supervisor Assignment Strategy

When a supervisor is added to the system, they receive an assignment record for each student in the program. When a new student is created, an assignment record is created for each active supervisor. This ensures supervisors are queryable through the same assignment-based access path as all other users, eliminating role-based branching in data access queries.

---

## 4. Permission Matrix

The following matrix defines which operations are available to each role and assignment level. All operations require an active assignment to the relevant student.

| Operation | Primary Teacher | Non-Primary Teacher | Paraeducator | Supervisor |
|---|---|---|---|---|
| View student profile | ✅ | ✅ | ✅ | ✅ |
| Edit student profile | ✅ | ❌ | ❌ | ❌ |
| Create goal | ✅ | ❌ | ❌ | ❌ |
| Edit goal | ✅ | ❌ | ❌ | ❌ |
| Archive goal | ✅ | ❌ | ❌ | ❌ |
| Add progress entry | ✅ | ✅ | ✅ | ❌ |
| Edit own progress entry | ✅ | ✅ | ✅ | ❌ |
| Edit others' progress entry | ✅ | ❌ | ❌ | ❌ |
| Delete own progress entry | ✅ | ✅ | ✅ | ❌ |
| Delete others' progress entry | ✅ | ❌ | ❌ | ❌ |
| Add critical note | ✅ | ✅ | ✅ | ❌ |
| View sensitive records | ✅ | ❌ | ❌ | ❌ |
| Generate report | ✅ | ✅ | ❌ | ✅ |

---

## 5. Authorization Architecture

### 5.1 Approach

The application uses ASP.NET Core's built-in policy-based and resource-based authorization framework. Authorization logic is centralized in handler classes rather than distributed across controllers or repositories.

There are two complementary authorization strategies:

- **Resource-based authorization** — used for single-entity endpoints. The controller loads or identifies the resource, then calls `IAuthorizationService.AuthorizeAsync` with a resource object. Registered handlers evaluate the user's role and assignment.
- **Query-scoped access** — used for list endpoints. The query itself joins through the `student_assignments` table so that unauthorized records never leave the database.

### 5.2 Operations

Operations represent the vocabulary of actions the system can authorize. They are defined as static fields on a central `Operations` class.

```csharp
public static class Operations
{
    public static readonly OperationAuthorizationRequirement ViewStudent =
        new() { Name = nameof(ViewStudent) };
    public static readonly OperationAuthorizationRequirement EditStudent =
        new() { Name = nameof(EditStudent) };
    public static readonly OperationAuthorizationRequirement CreateGoal =
        new() { Name = nameof(CreateGoal) };
    public static readonly OperationAuthorizationRequirement EditGoal =
        new() { Name = nameof(EditGoal) };
    public static readonly OperationAuthorizationRequirement ArchiveGoal =
        new() { Name = nameof(ArchiveGoal) };
    public static readonly OperationAuthorizationRequirement AddProgressEntry =
        new() { Name = nameof(AddProgressEntry) };
    public static readonly OperationAuthorizationRequirement EditProgressEntry =
        new() { Name = nameof(EditProgressEntry) };
    public static readonly OperationAuthorizationRequirement DeleteProgressEntry =
        new() { Name = nameof(DeleteProgressEntry) };
    public static readonly OperationAuthorizationRequirement AddCriticalNote =
        new() { Name = nameof(AddCriticalNote) };
    public static readonly OperationAuthorizationRequirement ViewSensitiveRecords =
        new() { Name = nameof(ViewSensitiveRecords) };
    public static readonly OperationAuthorizationRequirement GenerateReport =
        new() { Name = nameof(GenerateReport) };
}
```

### 5.3 Resource Objects

Resource objects are lightweight records that carry the context an authorization handler needs to make a decision. They are passed to `AuthorizeAsync` and routed to the appropriate handler by the framework's type-matching system.

```csharp
public record StudentResource(int StudentId);

public record ProgressEntryResource(int StudentId, int EntryId, int CreatedByUserId);

public record CriticalNoteResource(int StudentId, int NoteId, int CreatedByUserId, string? SensitivityLevel);
```

### 5.4 Authorization Handlers

#### 5.4.1 Student Authorization Handler

This handler evaluates all student-scoped operations. It loads the user's assignment and checks the `is_primary` flag and user role to determine access.

```csharp
public class StudentAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, StudentResource>
{
    private readonly IAssignmentRepository _assignments;

    public StudentAuthorizationHandler(IAssignmentRepository assignments)
    {
        _assignments = assignments;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        StudentResource resource)
    {
        var userId = context.User.GetUserId();
        var role = context.User.GetRole();

        var assignment = await _assignments.GetActiveAssignment(userId, resource.StudentId);
        if (assignment is null)
            return;

        switch (requirement.Name)
        {
            case nameof(Operations.ViewStudent):
                // Any active assignment grants view access
                context.Succeed(requirement);
                break;

            case nameof(Operations.EditStudent):
            case nameof(Operations.CreateGoal):
            case nameof(Operations.EditGoal):
            case nameof(Operations.ArchiveGoal):
            case nameof(Operations.ViewSensitiveRecords):
                // Only the primary teacher
                if (assignment.IsPrimary && role == Role.Teacher)
                    context.Succeed(requirement);
                break;

            case nameof(Operations.AddProgressEntry):
            case nameof(Operations.AddCriticalNote):
                // Teachers and paraeducators with any assignment
                if (role is Role.Teacher or Role.Paraeducator)
                    context.Succeed(requirement);
                break;

            case nameof(Operations.GenerateReport):
                // Teachers and supervisors
                if (role is Role.Teacher or Role.Supervisor)
                    context.Succeed(requirement);
                break;
        }
    }
}
```

#### 5.4.2 Progress Entry Authorization Handler

This handler evaluates entry-level ownership for edit and delete operations. It is called after the student-level check has already passed in the controller.

```csharp
public class ProgressEntryAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, ProgressEntryResource>
{
    private readonly IAssignmentRepository _assignments;

    public ProgressEntryAuthorizationHandler(IAssignmentRepository assignments)
    {
        _assignments = assignments;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        ProgressEntryResource resource)
    {
        var userId = context.User.GetUserId();
        var role = context.User.GetRole();

        switch (requirement.Name)
        {
            case nameof(Operations.EditProgressEntry):
            case nameof(Operations.DeleteProgressEntry):
                // Any author can edit or delete their own entry
                if (resource.CreatedByUserId == userId)
                {
                    context.Succeed(requirement);
                    return;
                }

                // Primary teachers can edit or delete anyone's entry
                // for their assigned students
                if (role == Role.Teacher)
                {
                    var assignment = await _assignments
                        .GetActiveAssignment(userId, resource.StudentId);
                    if (assignment is not null && assignment.IsPrimary)
                        context.Succeed(requirement);
                }
                break;
        }
    }
}
```

### 5.5 Handler Registration

```csharp
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthorizationHandler, StudentAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ProgressEntryAuthorizationHandler>();
builder.Services.AddScoped<IAssignmentRepository, CachedAssignmentRepository>();
```

---

## 6. Data Access Patterns

### 6.1 Single-Resource Endpoints

For endpoints that operate on a specific student, goal, or entry, the controller performs authorization before executing the operation.

```
Request → Extract resource identifier → AuthorizeAsync → Proceed or return 403
```

Example flow for updating a goal:

1. Extract `studentId` and `goalId` from the request.
2. Call `AuthorizeAsync(User, new StudentResource(studentId), Operations.EditGoal)`.
3. If authorization fails, return `403 Forbidden`.
4. Load the goal, validate it belongs to the student, and perform the update.

### 6.2 List Endpoints

For endpoints that return collections (e.g., "get my students"), the query joins through `student_assignments` to scope results to the current user. This ensures unauthorized records never leave the database.

```csharp
public async Task<IEnumerable<StudentSummaryDto>> GetAccessibleStudents(int userId)
{
    return await _db.QueryAsync<StudentSummaryDto>(
        @"SELECT s.id, s.identifier, s.program_year, s.age,
                 sa.is_primary
          FROM students s
          INNER JOIN student_assignments sa ON sa.student_id = s.id
          WHERE sa.user_id = @UserId
            AND sa.is_active = TRUE
            AND sa.start_date <= CURDATE()
            AND (sa.end_date IS NULL OR sa.end_date >= CURDATE())
            AND s.is_deleted = FALSE
          ORDER BY s.identifier",
        new { UserId = userId });
}
```

This query is role-agnostic. Supervisors, teachers, and paraeducators all use the same query. The assignments table determines what each user sees.

### 6.3 Assignment Caching

Because the assignment lookup is called on every single-resource authorization check, the repository is wrapped with a per-request cache to avoid redundant database queries within a single HTTP request.

```csharp
public class CachedAssignmentRepository : IAssignmentRepository
{
    private readonly IAssignmentRepository _inner;
    private readonly Dictionary<(int, int), StudentAssignment?> _cache = new();

    public CachedAssignmentRepository(IAssignmentRepository inner)
    {
        _inner = inner;
    }

    public async Task<StudentAssignment?> GetActiveAssignment(int userId, int studentId)
    {
        var key = (userId, studentId);
        if (_cache.TryGetValue(key, out var cached))
            return cached;

        var result = await _inner.GetActiveAssignment(userId, studentId);
        _cache[key] = result;
        return result;
    }
}
```

This is registered as a scoped service so the cache lives for the duration of one HTTP request.

---

## 7. Core Assignment Query

The core query used by both the authorization handlers and the cached repository:

```sql
SELECT id, user_id, student_id, is_primary, start_date, end_date, is_active
FROM student_assignments
WHERE user_id = @UserId
  AND student_id = @StudentId
  AND is_active = TRUE
  AND start_date <= CURDATE()
  AND (end_date IS NULL OR end_date >= CURDATE())
LIMIT 1;
```

This query enforces both the active flag and the date range, supporting time-bound assignments such as temporary coverage.

---

## 8. Controller Patterns

### 8.1 Single-Resource Authorization

```csharp
[HttpPut("{goalId}")]
public async Task<IActionResult> UpdateGoal(int studentId, int goalId, [FromBody] UpdateGoalRequest request)
{
    var authResult = await _auth.AuthorizeAsync(
        User, new StudentResource(studentId), Operations.EditGoal);
    if (!authResult.Succeeded) return Forbid();

    var goal = await _goals.GetById(goalId);
    if (goal is null || goal.StudentId != studentId) return NotFound();

    await _goals.Update(goalId, request, User.GetUserId());
    return NoContent();
}
```

### 8.2 Two-Layer Authorization (Student + Entry Ownership)

```csharp
[HttpPut("{entryId}")]
public async Task<IActionResult> UpdateEntry(int studentId, int entryId, [FromBody] UpdateEntryRequest request)
{
    // Layer 1: Can you access this student at all?
    var studentAuth = await _auth.AuthorizeAsync(
        User, new StudentResource(studentId), Operations.ViewStudent);
    if (!studentAuth.Succeeded) return Forbid();

    // Load the entry
    var entry = await _entries.GetById(entryId);
    if (entry is null || entry.StudentId != studentId) return NotFound();

    // Layer 2: Can you edit THIS entry specifically?
    var entryAuth = await _auth.AuthorizeAsync(
        User,
        new ProgressEntryResource(entry.StudentId, entry.Id, entry.CreatedByUserId),
        Operations.EditProgressEntry);
    if (!entryAuth.Succeeded) return Forbid();

    await _entries.Update(entryId, request, User.GetUserId());
    return NoContent();
}
```

### 8.3 List Endpoint (Query-Scoped)

```csharp
[HttpGet]
public async Task<IActionResult> GetMyStudents()
{
    var userId = User.GetUserId();
    var students = await _students.GetAccessibleStudents(userId);
    return Ok(students);
}
```

---

## 9. Sensitive Record Visibility

Records flagged as sensitive are only visible to users whose assignment has `is_primary = TRUE` and whose role is `Teacher`. This is enforced in two places:

- **Single-resource access**: The `ViewSensitiveRecords` operation is checked via `AuthorizeAsync` before returning sensitive content.
- **List queries**: Sensitive records are excluded from query results unless the current user meets the primary teacher criteria:

```sql
AND (pe.is_sensitive = FALSE OR sa.is_primary = TRUE)
```

---

## 10. Key Design Decisions

### 10.1 Assignments as the Single Source of Truth

All access decisions — whether evaluated in authorization handlers or embedded in SQL queries — derive from the `student_assignments` table. Roles determine the nature of permitted operations. Assignments determine the scope. This separation keeps the system predictable and auditable.

### 10.2 `is_primary` Over Assignment Type Enum

Rather than an enum of assignment types, the `is_primary` boolean provides a clear binary distinction: primary users have full control over a student's goals and records; non-primary users have limited, contributory access. The user's role combined with the `is_primary` flag covers all permission combinations described in the permission matrix.

### 10.3 Supervisors Modeled as Assignments

Supervisors receive explicit assignment records for each student. This avoids special-casing supervisor access in queries and handlers. Supervisors always have `is_primary = FALSE`, which naturally restricts them to read-only operations.

### 10.4 Two Authorization Strategies

Single-resource endpoints use `IAuthorizationService.AuthorizeAsync` with resource objects. List endpoints use query-scoped access via the assignments table. Both strategies use the same underlying assignment data, ensuring consistency.

---

## 11. Testing Strategy

### 11.1 Unit Tests

Each authorization handler should be unit tested in isolation by mocking the assignment repository and asserting `Succeed` or implicit denial for every combination of role, assignment status, `is_primary` flag, and operation.

### 11.2 Integration Tests

List queries should be integration tested to verify that they return only records the user is assigned to. A useful pattern is to compare the results of a list query against individual `AuthorizeAsync` calls for each returned record, asserting that they agree.

### 11.3 Consistency Audit

A periodic or on-demand audit job can iterate over list query results and verify that every returned record passes the corresponding `AuthorizeAsync` check. This catches drift between the two authorization strategies.