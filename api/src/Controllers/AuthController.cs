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

    [HttpPost("Login")]
    [ProducesResponseType(typeof(ResponseResult<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseResult<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResponseResult<LoginResponse>>> Login([FromBody] LoginDto login)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

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

        // Generate JWT access token
        var accessToken = _tokenService.GenerateToken(user.IdUser, user.Email!, user.RoleInternalName);

        // Generate refresh token secret
        var secretToken = Guid.NewGuid().ToString();
        var (refreshTokenHash, refreshTokenSalt) = PasswordHasher.HashPassword(secretToken);

        // Get device info
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var deviceInfo = JsonSerializer.Serialize(new { ip_address = ipAddress });

        // Store refresh token in database (30 days expiration)
        var refreshTokenId = await _authRepo.CreateRefreshTokenAsync(
            Guid.NewGuid(),
            user.IdUser,
            refreshTokenHash,
            refreshTokenSalt,
            expiresInSeconds: 2592000, // 30 days
            deviceInfo: deviceInfo,
            userAgent: userAgent
        );

        if (!refreshTokenId.HasValue)
        {
            return Ok(new ResponseResult<LoginResponse>
            {
                Success = false,
                Message = "Failed to create refresh token."
            });
        }

        // Build full refresh token: {id}.{secret}
        var fullRefreshToken = $"{refreshTokenId.Value}.{secretToken}";

        return Ok(new ResponseResult<LoginResponse>
        {
            Success = true,
            Message = "Login successful.",
            Data = new LoginResponse
            {
                UserId = user.IdUser,
                Email = user.Email!,
                Jwt = accessToken,
                RefreshToken = fullRefreshToken,
                Role = user.RoleInternalName,
                RoleDisplayName = user.RoleDisplayName
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

        // Split token into ID and secret: {id}.{secret}
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

        var tokenUser = await _userRepo.GetByIdAsync(matchedToken.IdUser);
        if (tokenUser == null)
        {
            return Unauthorized(new ResponseResult<TokenRefreshResponse>
            {
                Success = false,
                Message = "User not found."
            });
        }

        // Generate new JWT
        var newJwtToken = _tokenService.GenerateToken(tokenUser.IdUser, tokenUser.Email!, tokenUser.RoleInternalName);
        var jwtExpiresIn = _tokenService.GetTokenExpiryInSeconds(newJwtToken);

        // Generate new refresh token (rotation)
        var newSecretToken = Guid.NewGuid().ToString();
        var (newRefreshTokenHash, newRefreshTokenSalt) = PasswordHasher.HashPassword(newSecretToken);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var deviceInfo = JsonSerializer.Serialize(new { ip_address = ipAddress });

        var newRefreshTokenId = await _authRepo.ReplaceRefreshTokenAsync(
            matchedToken.IdRefreshToken,
            Guid.NewGuid(),
            tokenUser.IdUser,
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
