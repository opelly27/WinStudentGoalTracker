namespace WinStudentGoalTracker.Models;

public static class UserRoles
{
    // Role names from role-based-access-control.md
    public const string Teacher = "Teacher";
    public const string Paraeducator = "Paraeducator";
    public const string ProgramAdmin = "ProgramAdmin";
    public const string DistrictAdmin = "DistrictAdmin";
    public const string SuperAdmin = "SuperAdmin";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Teacher,
        Paraeducator,
        ProgramAdmin,
        DistrictAdmin,
        SuperAdmin
        
    };
}
