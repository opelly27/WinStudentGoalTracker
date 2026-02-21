namespace WinStudentGoalTracker.Models;

public class SelectProgramResponse
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
    public required string ProgramName { get; set; }
    public required string Jwt { get; set; }
    public required string RefreshToken { get; set; }
    public required string Role { get; set; }
    public required string RoleDisplayName { get; set; }
    public int JwtExpiresIn { get; set; }
}
