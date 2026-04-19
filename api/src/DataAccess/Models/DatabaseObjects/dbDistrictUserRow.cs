namespace WinStudentGoalTracker.DataAccess;

public class dbDistrictUserRow
{
    public Guid IdUser { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid IdProgram { get; set; }
    public string? ProgramName { get; set; }
    public Guid IdRole { get; set; }
    public string? RoleName { get; set; }
    public string? RoleInternalName { get; set; }
}
