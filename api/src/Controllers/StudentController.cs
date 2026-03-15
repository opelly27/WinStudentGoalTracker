using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.BaseClasses;
using WinStudentGoalTracker.DataAccess;
using WinStudentGoalTracker.Services;

namespace WinStudentGoalTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : BaseController
{
    private readonly StudentRepository _studentRepository = new();


    [HttpGet("my")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Paraeducator},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<IEnumerable<StudentResponse>>>> GetMyStudents()
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();

        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        return Ok(new ResponseResult<IEnumerable<StudentResponse>>
        {
            Success = true,
            Message = "Students retrieved successfully.",
            Data = students
        });
    }


    // TODO refactor with database changes to ensure
    // users who are a district admin are actually associated with a district, and
    // then this endpoint should validate that the requested program is part of the district
    // Once that is in place, then district admins will be allowed to call this function.
    [HttpGet("program/{idProgram:guid}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<IEnumerable<StudentResponse>>>> GetStudentsForProgram(Guid idProgram)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();

        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, idProgram, role);

        return Ok(new ResponseResult<IEnumerable<StudentResponse>>
        {
            Success = true,
            Message = "Students retrieved successfully.",
            Data = students
        });
    }

    [HttpGet("{idStudent:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Paraeducator},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> GetById(Guid idStudent)
    {

        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var student = students.Single(s => s.StudentId == idStudent);

        return Ok(new ResponseResult<StudentResponse>
        {
            Success = true,
            Message = "Student retrieved successfully.",
            Data = student
        });
    }

    [HttpGet("{idStudent:guid}/goals")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Paraeducator},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<StudentGoalSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentGoalSummary>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentGoalSummary>>> GetGoals(Guid idStudent)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentGoalSummary>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var summary = await _studentRepository.GetGoalSummaryAsync(idStudent);

        return Ok(new ResponseResult<StudentGoalSummary>
        {
            Success = true,
            Message = "Goals retrieved successfully.",
            Data = summary
        });
    }

    [HttpGet("{idStudent:guid}/benchmarks")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Paraeducator},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<StudentBenchmarkSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentBenchmarkSummary>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentBenchmarkSummary>>> GetBenchmarks(Guid idStudent)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentBenchmarkSummary>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var summary = await _studentRepository.GetBenchmarkSummaryAsync(idStudent);

        return Ok(new ResponseResult<StudentBenchmarkSummary>
        {
            Success = true,
            Message = "Benchmarks retrieved successfully.",
            Data = summary
        });
    }

    [HttpPost("{idStudent:guid}/goals")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<StudentGoalItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult<StudentGoalItem>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseResult<StudentGoalItem>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<StudentGoalItem>>> CreateGoal(Guid idStudent, [FromBody] CreateGoalDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentGoalItem>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        if (!PermissionService.IsAllowed(role, EntityType.Goal, PermissionAction.Create, isMine: true))
        {
            return BadRequest(new ResponseResult<StudentGoalItem>
            {
                Success = false,
                Message = "Unable to create goal. - Permission Matrix"
            });
        }

        if (dto.GoalParentId.HasValue)
        {
            var summary = await _studentRepository.GetGoalSummaryAsync(idStudent);
            var parentGoal = summary?.Goals.FirstOrDefault(g => g.GoalId == dto.GoalParentId.Value);

            if (parentGoal is null)
            {
                return BadRequest(new ResponseResult<StudentGoalItem>
                {
                    Success = false,
                    Message = "Parent goal not found."
                });
            }

            if (parentGoal.GoalParentId.HasValue)
            {
                return BadRequest(new ResponseResult<StudentGoalItem>
                {
                    Success = false,
                    Message = "The selected parent goal already has a parent."
                });
            }
        }

        var created = await _studentRepository.InsertGoalAsync(idStudent, userId, dto);
        if (created is null)
        {
            return BadRequest(new ResponseResult<StudentGoalItem>
            {
                Success = false,
                Message = "Unable to create goal."
            });
        }

        return StatusCode(StatusCodes.Status201Created, new ResponseResult<StudentGoalItem>
        {
            Success = true,
            Message = "Goal created successfully.",
            Data = created
        });
    }

    [HttpPut("{idStudent:guid}/goals/{idGoal:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<object>>> UpdateGoal(Guid idStudent, Guid idGoal, [FromBody] UpdateGoalDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var updated = await _studentRepository.UpdateGoalAsync(idGoal, dto);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = updated ? "Goal updated successfully." : "No changes were applied."
        });
    }

    [HttpPost("{idStudent:guid}/progress-event")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Paraeducator},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult>> AddProgressEvent(Guid idStudent, [FromBody] AddProgressEventDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var created = await _studentRepository.AddProgressEventAsync(userId, dto);
        if (!created)
        {
            return BadRequest(new ResponseResult
            {
                Success = false,
                Message = "Unable to add progress event."
            });
        }

        return StatusCode(StatusCodes.Status201Created, new ResponseResult
        {
            Success = true,
            Message = "Progress event added successfully."
        });
    }

    [HttpGet("goals/{idGoal:guid}/progress-events")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.Paraeducator},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<ProgressEventResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<ProgressEventResponse>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<IEnumerable<ProgressEventResponse>>>> GetProgressEventsForGoal(Guid idGoal)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var studentId = await _studentRepository.GetStudentIdForGoalAsync(idGoal);
        if (!studentId.HasValue)
        {
            return NotFound(new ResponseResult<IEnumerable<ProgressEventResponse>>
            {
                Success = false,
                Message = "Goal not found."
            });
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(studentId.Value))
        {
            return NotFound(new ResponseResult<IEnumerable<ProgressEventResponse>>
            {
                Success = false,
                Message = "Goal not found."
            });
        }

        var progressEvents = await _studentRepository.GetProgressEventsForGoalAsync(idGoal);

        return Ok(new ResponseResult<IEnumerable<ProgressEventResponse>>
        {
            Success = true,
            Message = "Progress events retrieved successfully.",
            Data = progressEvents
        });
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Teacher}")]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> CreateStudent([FromBody] CreateStudentDto newStudentData)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();

        if (error is not null)
        {
            return error;
        }

        if (!PermissionService.IsAllowed(role, EntityType.Student, PermissionAction.Create))
        {
            return BadRequest(new ResponseResult
            {
                Success = false,
                Message = "Unable to create student."
            });
        }

        var newStudentId = Guid.NewGuid();
        var created = await _studentRepository.InsertAsync(newStudentData, newStudentId, programId, userId);
        if (created is null)
        {
            return BadRequest(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Unable to create student."
            });
        }

        return CreatedAtAction(nameof(GetById), new { idStudent = created.StudentId }, new ResponseResult<StudentResponse>
        {
            Success = true,
            Message = "Student created successfully.",
            Data = created
        });
    }

    [HttpPut("{idStudent:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher}")]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> UpdateStudent(Guid idStudent, [FromBody] UpdateStudentDto request)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var updated = await _studentRepository.UpdateAsync(idStudent, request);
        var refreshed = await _studentRepository.GetByIdAsync(idStudent);
        if (refreshed is null)
        {
            return NotFound(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Student not found after update."
            });
        }

        return Ok(new ResponseResult<StudentResponse>
        {
            Success = true,
            Message = updated ? "Changes applied successfully." : "No changes were applied.",
            Data = refreshed
        });
    }

    [HttpDelete("{idStudent:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<object>>> Delete(Guid idStudent)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var deleted = await _studentRepository.DeleteAsync(idStudent);
        if (!deleted)
        {
            return NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = "Student deleted."
        });
    }

    [HttpPost("{idStudent:guid}/benchmarks")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<StudentBenchmarkItem>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult<StudentBenchmarkItem>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseResult<StudentBenchmarkItem>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<StudentBenchmarkItem>>> CreateBenchmark(Guid idStudent, [FromBody] CreateBenchmarkDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentBenchmarkItem>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        if (!PermissionService.IsAllowed(role, EntityType.Benchmark, PermissionAction.Create, isMine: true))
        {
            return BadRequest(new ResponseResult<StudentBenchmarkItem>
            {
                Success = false,
                Message = "Unable to create benchmark. - Permission Matrix"
            });
        }

        var created = await _studentRepository.InsertBenchmarkAsync(dto.GoalId, userId, dto);
        if (created is null)
        {
            return BadRequest(new ResponseResult<StudentBenchmarkItem>
            {
                Success = false,
                Message = "Unable to create benchmark."
            });
        }

        return StatusCode(StatusCodes.Status201Created, new ResponseResult<StudentBenchmarkItem>
        {
            Success = true,
            Message = "Benchmark created successfully.",
            Data = created
        });
    }

    [HttpPut("{idStudent:guid}/benchmarks/{idBenchmark:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<object>>> UpdateBenchmark(Guid idStudent, Guid idBenchmark, [FromBody] UpdateBenchmarkDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.StudentId).Contains(idStudent))
        {
            return NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        var updated = await _studentRepository.UpdateBenchmarkAsync(idBenchmark, dto);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = updated ? "Changes applied successfully." : "No changes were applied."
        });
    }
}
