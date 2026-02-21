namespace WinStudentGoalTracker.Models;

public class TokenRefreshResponse
{
    public required string Jwt { get; set; }
    public required string NewRefreshToken { get; set; }
    public int JwtExpiresIn { get; set; }
}
