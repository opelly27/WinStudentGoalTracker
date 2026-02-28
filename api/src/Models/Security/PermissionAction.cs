namespace WinStudentGoalTracker.Models;

public static class PermissionAction
{
    public const string Create = "create";
    public const string Read   = "read";
    public const string Update = "update";
    public const string Delete = "delete";

    public static string? TryParse(string value) =>
        All.Contains(value) ? value : null;

    public static readonly IReadOnlyList<string> All =
        [Create, Read, Update, Delete];
}
