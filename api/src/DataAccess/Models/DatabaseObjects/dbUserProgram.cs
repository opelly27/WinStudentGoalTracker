namespace WinStudentGoalTracker.DataAccess;

public class dbUserProgram
{
    public required Guid IdProgram { get; set; }
    public string? ProgramName { get; set; }
    public required string RoleInternalName { get; set; }
    public required string RoleDisplayName { get; set; }
    public bool IsPrimary { get; set; }
    public string? Status { get; set; }
}
