using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinStudentGoalTracker.BaseClasses;
using WinStudentGoalTracker.DataAccess;
using WinStudentGoalTracker.Models;
using WinStudentGoalTracker.Services;

namespace WinStudentGoalTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly UserRepository _userRepo = new();
    private readonly AuthRepository _authRepo = new();
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    // Phase 1: verify credentials, return session token + list of programs
    [HttpPost("Login")]
    [ProducesResponseType(typeof(ResponseResult<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<LoginResponse>>> Login([FromBody] LoginDto login)
    {
        if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Password))
        {
            return BadRequest(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "Email and password are required."
            });
        }

        var user = await _userRepo.GetByEmailAsync(login.Email);
        if (user == null)
        {
            return Ok(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "Invalid email or password."
            });
        }

        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
        {
            return Ok(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "Account is temporarily locked. Please try again later."
            });
        }

        if (user.PasswordHash == null || user.PasswordSalt == null)
        {
            return Ok(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "Password not set. Please contact an administrator."
            });
        }

        if (!PasswordHasher.VerifyPassword(login.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Ok(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "Invalid email or password."
            });
        }

        var programs = await _userRepo.GetProgramsForUserIdAsync(user.IdUser);
        var programList = programs.ToList();

        if (programList.Count == 0)
        {
            return Ok(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "No active programs found for this account."
            });
        }

        var sessionToken = _tokenService.GenerateSessionToken(user.IdUser, user.Email!);

        return Ok(new ResponseResult<LoginResponse>
        {
            Success = true,
            Message = "Login successful.",
            Data = new LoginResponse
            {
                SessionToken = sessionToken,
                Programs = programList.Select(p => new UserProgramSummary
                {
                    ProgramId = p.IdProgram,
                    ProgramName = p.ProgramName!,
                    Role = p.RoleInternalName,
                    RoleDisplayName = p.RoleDisplayName,
                    IsPrimary = p.IsPrimary
                }).ToList()
            }
        });
    }

    // Phase 2: user selects a program, receive program-scoped JWT + refresh token
    // Requires the phase 1 session token in the Authorization: Bearer header
    [HttpPost("SelectProgram")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseResult<SelectProgramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<SelectProgramResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseResult<SelectProgramResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseResult<SelectProgramResponse>>> SelectProgram([FromBody] SelectProgramDto dto)
    {
        var authStage = User.FindFirst("auth_stage")?.Value;
        if (authStage != "selecting_program")
        {
            return Unauthorized(new ResponseResult<SelectProgramResponse>
            {
                Success = false,
                Message = "A session token is required to select a program."
            });
        }

        var (userId, userIdError) = GetUserIdFromClaims();
        if (userIdError != null) return userIdError;

        if (!Guid.TryParse(dto.ProgramId, out Guid programId))
        {
            return BadRequest(new ResponseResult<SelectProgramResponse>
            {
                Success = false,
                Message = "Invalid program ID format."
            });
        }

        var programUser = await _userRepo.GetByIdWithProgramAsync(userId, programId);
        if (programUser == null)
        {
            return Unauthorized(new ResponseResult<SelectProgramResponse>
            {
                Success = false,
                Message = "User does not have access to this program."
            });
        }

        if (programUser.Status != "active")
        {
            return Unauthorized(new ResponseResult<SelectProgramResponse>
            {
                Success = false,
                Message = "Access to this program is inactive."
            });
        }

        var accessToken = _tokenService.GenerateToken(
            programUser.IdUser,
            programUser.Email!,
            programUser.RoleInternalName,
            programUser.IdProgram);

        var jwtExpiresIn = _tokenService.GetTokenExpiryInSeconds(accessToken);

        var refreshToken = Guid.NewGuid().ToString();
        var (refreshTokenHash, refreshTokenSalt) = PasswordHasher.HashPassword(refreshToken);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var deviceInfo = JsonSerializer.Serialize(new { ip_address = ipAddress });

        var refreshTokenId = await _authRepo.CreateRefreshTokenAsync(
            Guid.NewGuid(),
            programUser.IdUser,
            programUser.IdProgram,
            refreshTokenHash,
            refreshTokenSalt,
            expiresInSeconds: 2592000, // 30 days
            deviceInfo: deviceInfo,
            userAgent: userAgent
        );

        if (!refreshTokenId.HasValue)
        {
            return Ok(new ResponseResult<SelectProgramResponse>
            {
                Success = false,
                Message = "Failed to create refresh token."
            });
        }

        var fullRefreshToken = $"{refreshTokenId.Value}.{refreshToken}";

        return Ok(new ResponseResult<SelectProgramResponse>
        {
            Success = true,
            Message = "Program selected.",
            Data = new SelectProgramResponse
            {
                UserId = programUser.IdUser,
                Email = programUser.Email!,
                ProgramName = programUser.ProgramName!,
                Jwt = accessToken,
                RefreshToken = fullRefreshToken,
                Role = programUser.RoleInternalName,
                RoleDisplayName = programUser.RoleDisplayName,
                JwtExpiresIn = jwtExpiresIn
            }
        });
    }

    [HttpPost("RefreshToken")]
    [ProducesResponseType(typeof(ResponseResult<TokenRefreshResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<TokenRefreshResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseResult<TokenRefreshResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResponseResult<TokenRefreshResponse>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
        {
            return BadRequest(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Refresh token is required."
            });
        }

        var dotIndex = refreshTokenDto.RefreshToken.IndexOf('.');
        if (dotIndex < 1)
        {
            return BadRequest(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Invalid refresh token format."
            });
        }

        var tokenIdStr = refreshTokenDto.RefreshToken[..dotIndex];
        var secretToken = refreshTokenDto.RefreshToken[(dotIndex + 1)..];

        if (!Guid.TryParse(tokenIdStr, out Guid tokenId))
        {
            return BadRequest(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Invalid refresh token ID."
            });
        }

        var matchedToken = await _authRepo.GetRefreshTokenByIdAsync(tokenId);
        if (matchedToken == null)
        {
            return Unauthorized(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Invalid refresh token."
            });
        }

        if (!PasswordHasher.VerifyPassword(secretToken, matchedToken.TokenHash, matchedToken.TokenSalt))
        {
            return Unauthorized(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Invalid refresh token."
            });
        }

        if (matchedToken.ExpiresAt < DateTime.UtcNow)
        {
            return Unauthorized(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Refresh token has expired."
            });
        }

        if (matchedToken.RevokedAt.HasValue)
        {
            return Unauthorized(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Refresh token has been revoked."
            });
        }

        // Use program-scoped lookup so the new JWT carries current role + program
        var programUser = await _userRepo.GetByIdWithProgramAsync(matchedToken.IdUser, matchedToken.IdProgram);
        if (programUser == null)
        {
            return Unauthorized(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "User or program not found."
            });
        }

        var newJwtToken = _tokenService.GenerateToken(
            programUser.IdUser,
            programUser.Email!,
            programUser.RoleInternalName,
            programUser.IdProgram);

        var jwtExpiresIn = _tokenService.GetTokenExpiryInSeconds(newJwtToken);

        var newSecretToken = Guid.NewGuid().ToString();
        var (newRefreshTokenHash, newRefreshTokenSalt) = PasswordHasher.HashPassword(newSecretToken);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var deviceInfo = JsonSerializer.Serialize(new { ip_address = ipAddress });

        var newRefreshTokenId = await _authRepo.ReplaceRefreshTokenAsync(
            matchedToken.IdRefreshToken,
            Guid.NewGuid(),
            programUser.IdUser,
            programUser.IdProgram,
            newRefreshTokenHash,
            newRefreshTokenSalt,
            expiresInSeconds: 2592000,
            deviceInfo: deviceInfo,
            userAgent: userAgent
        );

        if (!newRefreshTokenId.HasValue)
        {
            return Ok(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "Failed to create new refresh token."
            });
        }

        var fullNewRefreshToken = $"{newRefreshTokenId.Value}.{newSecretToken}";

        return Ok(new ResponseResult<TokenRefreshResponse>
        {
            Success = true,
            Message = "Token refreshed successfully.",
            Data = new TokenRefreshResponse
            {
                Jwt = newJwtToken,
                NewRefreshToken = fullNewRefreshToken,
                JwtExpiresIn = jwtExpiresIn
            }
        });
    }

    [HttpPost("Logout")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<object>>> Logout([FromBody] RefreshTokenDto logoutDto)
    {
        if (string.IsNullOrWhiteSpace(logoutDto.RefreshToken))
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Refresh token is required."
            });
        }

        var (userId, error) = GetUserIdFromClaims();
        if (error != null) return error;

        var dotIndex = logoutDto.RefreshToken.IndexOf('.');
        if (dotIndex < 1 || !Guid.TryParse(logoutDto.RefreshToken[..dotIndex], out Guid tokenId))
        {
            return BadRequest(new ResponseResult<object>
            {
                Success = false,
                Message = "Invalid refresh token format."
            });
        }

        var tokenData = await _authRepo.GetRefreshTokenByIdAsync(tokenId);
        if (tokenData == null || tokenData.IdUser != userId)
        {
            return Unauthorized(new ResponseResult<object>
            {
                Success = false,
                Message = "Invalid refresh token."
            });
        }

        if (tokenData.RevokedAt.HasValue)
        {
            return Ok(new ResponseResult<object>
            {
                Success = true,
                Message = "Already logged out."
            });
        }

        await _authRepo.RevokeRefreshTokenAsync(tokenId);

        return Ok(new ResponseResult<object>
        {
            Success = true,
            Message = "Logged out successfully."
        });
    }
}
