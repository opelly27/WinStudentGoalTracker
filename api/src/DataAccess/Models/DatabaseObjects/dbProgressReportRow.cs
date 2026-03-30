namespace WinStudentGoalTracker.DataAccess;

public class dbProgressReportRow
{
    public required Guid GoalId { get; set; }
    public required Guid ProgressEventId { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? BenchmarkNames { get; set; }
}
