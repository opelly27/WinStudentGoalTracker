namespace WinStudentGoalTracker.DataAccess;

public class AdminCreateUserDto
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? ProgramId { get; set; }
    public string? RoleId { get; set; }
}
