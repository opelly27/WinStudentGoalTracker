namespace WinStudentGoalTracker.Models;

public class StudentFullProfileResponse
{
    public required StudentResponse Student { get; set; }
    public List<StudentGoalItem> Goals { get; set; } = [];
    public List<StudentBenchmarkItem> Benchmarks { get; set; } = [];
    public List<ProgressEventWithGoalResponse> ProgressEvents { get; set; } = [];
    public List<ProgressEventBenchmarkLink> ProgressEventBenchmarks { get; set; } = [];
}

public class ProgressEventWithGoalResponse
{
    public Guid ProgressEventId { get; set; }
    public Guid GoalId { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}

public class ProgressEventBenchmarkLink
{
    public Guid ProgressEventId { get; set; }
    public Guid BenchmarkId { get; set; }
}
