using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.Models.ResponseTypes;
using WinStudentGoalTracker.BaseClasses;
using WinStudentGoalTracker.DataAccess;

namespace WinStudentGoalTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportPromptController : BaseController
{
    // ************************** Constructor **************************
    private readonly ReportPromptRepository _reportPromptRepository;

    public ReportPromptController()
    {
        _reportPromptRepository = new();
    }

    // ************************ Public Methods *************************

    [HttpGet]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<IEnumerable<ReportPromptResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseResult<IEnumerable<ReportPromptResponse>>>> GetAll()
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var prompts = await _reportPromptRepository.GetAllAsync();

        return Ok(new ResponseResult<IEnumerable<ReportPromptResponse>>
        {
            Success = true,
            Message = "Report prompts retrieved successfully.",
            Data = prompts
        });
    }

    [HttpGet("{idReportPrompt:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<ReportPromptResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<ReportPromptResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<ReportPromptResponse>>> GetById(Guid idReportPrompt)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var prompt = await _reportPromptRepository.GetByIdAsync(idReportPrompt);
        if (prompt is null)
        {
            return NotFound(new ResponseResult<ReportPromptResponse>
            {
                Success = false,
                Message = "Report prompt not found."
            });
        }

        return Ok(new ResponseResult<ReportPromptResponse>
        {
            Success = true,
            Message = "Report prompt retrieved successfully.",
            Data = prompt
        });
    }

    // *****************************************************************
    // Returns the report prompt for the given reportname scoped to
    // the authenticated user's program.
    // *****************************************************************
    [HttpGet("by-name/{reportname}")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<ReportPromptResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<ReportPromptResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<ReportPromptResponse>>> GetByReportname(string reportname)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var prompt = await _reportPromptRepository.GetByReportnameAsync(reportname, programId);
        if (prompt is null)
        {
            return NotFound(new ResponseResult<ReportPromptResponse>
            {
                Success = false,
                Message = "Report prompt not found."
            });
        }

        return Ok(new ResponseResult<ReportPromptResponse>
        {
            Success = true,
            Message = "Report prompt retrieved successfully.",
            Data = prompt
        });
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<ReportPromptResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseResult<ReportPromptResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<ReportPromptResponse>>> Create([FromBody] CreateReportPromptDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        // Scope the new prompt to the authenticated user's program.
        dto.ProgramId = programId.ToString();

        var created = await _reportPromptRepository.InsertAsync(dto);
        if (created is null)
        {
            return BadRequest(new ResponseResult<ReportPromptResponse>
            {
                Success = false,
                Message = "Unable to create report prompt."
            });
        }

        return StatusCode(StatusCodes.Status201Created, new ResponseResult<ReportPromptResponse>
        {
            Success = true,
            Message = "Report prompt created successfully.",
            Data = created
        });
    }

    [HttpPut("{idReportPrompt:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<object>>> Update(Guid idReportPrompt, [FromBody] UpdateReportPromptDto dto)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var updated = await _reportPromptRepository.UpdateAsync(idReportPrompt, dto);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = updated ? "Report prompt updated successfully." : "No changes were applied."
        });
    }

    [HttpDelete("{idReportPrompt:guid}")]
    [Authorize(Roles = $"{UserRoles.Teacher},{UserRoles.ProgramAdmin},{UserRoles.SuperAdmin}")]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseResult<object>>> Delete(Guid idReportPrompt)
    {
        var (userId, email, programId, role, error) = GetProgramUserFromClaims();
        if (error is not null)
        {
            return error;
        }

        var deleted = await _reportPromptRepository.DeleteAsync(idReportPrompt);
        if (!deleted)
        {
            return NotFound(new ResponseResult<object>
            {
                Success = false,
                Message = "Report prompt not found."
            });
        }

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = "Report prompt deleted."
        });
    }
}
