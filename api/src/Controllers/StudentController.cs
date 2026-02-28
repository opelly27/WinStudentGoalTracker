using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.BaseClasses;
using WinStudentGoalTracker.DataAccess;

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
        var response = students.Select(StudentResponse.FromDatabaseModel);

        return Ok(new ResponseResult<IEnumerable<StudentResponse>>
        {
            Success = true,
            Data = response
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

        var students = await _studentRepository.GetStudentsByProgramAsync(idProgram);
        var response = students.Select(StudentResponse.FromDatabaseModel);

        return Ok(new ResponseResult<IEnumerable<StudentResponse>>
        {
            Success = true,
            Data = response
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

        if (!students.Select(s => s.IdStudent).Contains(idStudent))
        {
            return NotFound(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Student not found."
            });
        }
        
        var student = students.Single(s => s.IdStudent == idStudent);

        return Ok(new ResponseResult<StudentResponse>
        {
            Success = true,
            Data = StudentResponse.FromDatabaseModel(student)
        });
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Teacher}")]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> Create([FromBody] CreateStudentDto newStudentData)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();

        if (error is not null)
        {
            return error;
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

        var response = StudentResponse.FromDatabaseModel(created);
        return CreatedAtAction(nameof(GetById), new { idStudent = response.IdStudent }, new ResponseResult<StudentResponse>
        {
            Success = true,
            Data = response
        });
    }

    [HttpPut("{idStudent:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher}")]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> Update(Guid idStudent, [FromBody] UpdateStudentDto request)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetMyStudentsAsync(userId, programId, role);

        if (!students.Select(s => s.IdStudent).Contains(idStudent))
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
            Message = updated ? null : "No changes were applied.",
            Data = StudentResponse.FromDatabaseModel(refreshed)
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

        if (!students.Select(s => s.IdStudent).Contains(idStudent))
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
}
