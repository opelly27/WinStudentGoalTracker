namespace WinStudentGoalTracker.Models;

public class StudentResponse
{
    public Guid StudentId { get; set; }
    public string? Identifier { get; set; }
    public DateTime? ExpectedGradDate { get; set; }
    public DateTime? LastEntryDate { get; set; }
    public int GoalCount { get; set; }
    public int ProgressEventCount { get; set; }
}
