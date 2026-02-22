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


    // TODO refactor this stored procedure
    // to getmystudents
    // This required auth system to be set up first
    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<StudentResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<IEnumerable<StudentResponse>>>> GetAll()
    {

        var (userId, email, programId, role, error) = GetProgramUserFromClaims();

        if (error is not null)
        {
            return error;
        }

        var students = await _studentRepository.GetAllAsync();
        var response = students.Select(StudentResponse.FromDatabaseModel);

        return Ok(new ResponseResult<IEnumerable<StudentResponse>>
        {
            Success = true,
            Data = response
        });
    }

    [HttpGet("{idStudent:guid}")]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> GetById(Guid idStudent)
    {
        var student = await _studentRepository.GetByIdAsync(idStudent);
        if (student is null)
        {
            return NotFound(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = "Student not found."
            });
        }

        return Ok(new ResponseResult<StudentResponse>
        {
            Success = true,
            Data = StudentResponse.FromDatabaseModel(student)
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> Create([FromBody] CreateStudentDto request)
    {
        var existing = await _studentRepository.GetByIdAsync(request.IdStudent);
        if (existing is not null)
        {
            return BadRequest(new ResponseResult<StudentResponse>
            {
                Success = false,
                Message = $"Student with id {request.IdStudent} already exists."
            });
        }

        var created = await _studentRepository.InsertAsync(request);
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
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<StudentResponse>>> Update(Guid idStudent, [FromBody] UpdateStudentDto request)
    {
        var existing = await _studentRepository.GetByIdAsync(idStudent);
        if (existing is null)
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
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<object>>> Delete(Guid idStudent)
    {
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
