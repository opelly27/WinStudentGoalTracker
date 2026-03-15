namespace WinStudentGoalTracker.DataAccess;

public class dbStudentBenchmarkRow
{
    public string? StudentIdentifier { get; set; }
    public required Guid BenchmarkId { get; set; }
    public required Guid GoalId { get; set; }
    public string? GoalCategory { get; set; }
    public string? Benchmark { get; set; }
    public string? ShortName { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
