namespace WinStudentGoalTracker.Models;

public class StudentGoalItem
{
    public Guid GoalId { get; set; }
    public Guid? GoalParentId { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Baseline { get; set; }
    public int ProgressEventCount { get; set; }
    public int BenchmarkCount { get; set; }
}
