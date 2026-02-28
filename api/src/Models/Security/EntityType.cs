namespace WinStudentGoalTracker.Models;

public static class EntityType
{
    public const string SchoolDistrict = "school_district";
    public const string Program        = "program";
    public const string User           = "user";
    public const string Student        = "student";
    public const string Goal           = "goal";
    public const string ProgressEvent  = "progress_event";

    public static string? TryParse(string value) =>
        All.Contains(value) ? value : null;

    public static readonly IReadOnlyList<string> All =
        [SchoolDistrict, Program, User, Student, Goal, ProgressEvent];
}
