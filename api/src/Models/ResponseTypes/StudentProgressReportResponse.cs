namespace WinStudentGoalTracker.Models;

public class StudentProgressReportResponse
{
    public string? StudentIdentifier { get; set; }
    public List<ProgressReportGoal> Goals { get; set; } = [];
}

public class ProgressReportGoal
{
    public Guid GoalId { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public List<ProgressReportEvent> ProgressEvents { get; set; } = [];
}

public class ProgressReportEvent
{
    public Guid ProgressEventId { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? BenchmarkNames { get; set; }
}
