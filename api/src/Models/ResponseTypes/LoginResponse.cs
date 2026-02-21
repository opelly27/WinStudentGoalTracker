namespace WinStudentGoalTracker.Models;

public class LoginResponse
{
    public int UserId { get; set; }
    public required string Email { get; set; }
    public required string Jwt { get; set; }
    public required string RefreshToken { get; set; }
    public string? Role { get; set; }
}
