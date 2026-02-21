namespace WinStudentGoalTracker.DataAccess;

public class dbProgramUser
{
    public required Guid IdUser { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public required Guid IdProgram { get; set; }
    public string? ProgramName { get; set; }
    public required string RoleInternalName { get; set; }
    public required string RoleDisplayName { get; set; }
    public string? Status { get; set; }
}
