namespace WinStudentGoalTracker.Models;

public class LoginResponse
{
    public required string SessionToken { get; set; }
    public required List<UserProgramSummary> Programs { get; set; }
}

public class UserProgramSummary
{
    public Guid ProgramId { get; set; }
    public required string ProgramName { get; set; }
    public required string Role { get; set; }
    public required string RoleDisplayName { get; set; }
    public bool IsPrimary { get; set; }
}
