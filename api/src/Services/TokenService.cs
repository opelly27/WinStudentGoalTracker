using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WinStudentGoalTracker.Models;

namespace WinStudentGoalTracker.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly int _tokenExpiryInSeconds = 60 * 15; // 15 minutes

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(Guid userId, string email, string role)
    {

        if (UserRoles.TryParse(role) is null)
        {
            throw new ArgumentException("Invalid role name");
        }

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("user_id", userId.ToString())
        };

        if (role is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_tokenExpiryInSeconds),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int GetTokenExpiryInSeconds(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var expiryTime = jwtToken.ValidTo;
            var currentTime = DateTime.UtcNow;

            var timeUntilExpiry = expiryTime - currentTime;

            return timeUntilExpiry.TotalSeconds > 0 ? (int)timeUntilExpiry.TotalSeconds : 0;
        }
        catch
        {
            return 0;
        }
    }
}
