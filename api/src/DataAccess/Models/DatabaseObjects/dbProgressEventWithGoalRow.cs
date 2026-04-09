namespace WinStudentGoalTracker.DataAccess;

public class dbProgressEventWithGoalRow
{
    public required Guid ProgressEventId { get; set; }
    public required Guid GoalId { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
