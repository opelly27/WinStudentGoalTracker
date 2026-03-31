namespace WinStudentGoalTracker.Models;

public class StudentResponse
{
    public Guid StudentId { get; set; }
    public string? Identifier { get; set; }
    public DateTime? NextIepDate { get; set; }
    public DateTime? FirstEntryDate { get; set; }
    public DateTime? LastEntryDate { get; set; }
    public int GoalCount { get; set; }
    public int ProgressEventCount { get; set; }
    public int BenchmarkCount { get; set; }
}
