namespace WinStudentGoalTracker.DataAccess;

public class dbProgressEventRow
{
    public required Guid ProgressEventId { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
