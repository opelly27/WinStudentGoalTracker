namespace WinStudentGoalTracker.DataAccess;

public class dbStudentGoalRow
{
    public string? StudentIdentifier { get; set; }
    public required Guid GoalId { get; set; }
    public Guid? GoalParentId { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Baseline { get; set; }
    public DateTime? TargetCompletionDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public bool? Achieved { get; set; }
    public string? CloseNotes { get; set; }
    public int ProgressEventCount { get; set; }
    public int BenchmarkCount { get; set; }
}
