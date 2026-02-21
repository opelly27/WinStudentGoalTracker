namespace WinStudentGoalTracker.Models;

public static class UserRoles
{
    public const string Teacher       = "teacher";
    public const string Paraeducator  = "paraeducator";
    public const string ProgramAdmin  = "program_admin";
    public const string DistrictAdmin = "district_admin";
    public const string SuperAdmin    = "super_admin";

    public static string? TryParse(string value) =>
        All.Contains(value) ? value : null;

    public static readonly IReadOnlyList<string> All =
        [Teacher, Paraeducator, ProgramAdmin, DistrictAdmin, SuperAdmin];
}
