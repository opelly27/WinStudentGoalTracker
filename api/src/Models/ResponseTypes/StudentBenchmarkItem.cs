namespace WinStudentGoalTracker.Models;

public class StudentBenchmarkItem
{
    public Guid BenchmarkId { get; set; }
    public Guid GoalId { get; set; }
    public string? GoalCategory { get; set; }
    public string? Benchmark { get; set; }
    public string? ShortName { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
