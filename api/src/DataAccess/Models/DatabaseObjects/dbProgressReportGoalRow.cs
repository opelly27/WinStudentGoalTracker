namespace WinStudentGoalTracker.DataAccess;

public class dbProgressReportGoalRow
{
    public required Guid GoalId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
