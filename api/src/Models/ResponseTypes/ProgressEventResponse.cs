namespace WinStudentGoalTracker.Models;

public class ProgressEventResponse
{
    public Guid ProgressEventId { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}
