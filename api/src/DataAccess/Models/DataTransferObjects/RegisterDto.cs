namespace WinStudentGoalTracker.DataAccess;

public class RegisterDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? DistrictName { get; set; }
    public string? DistrictContactEmail { get; set; }
    public string? ProgramName { get; set; }
    public string? ProgramDescription { get; set; }
}
