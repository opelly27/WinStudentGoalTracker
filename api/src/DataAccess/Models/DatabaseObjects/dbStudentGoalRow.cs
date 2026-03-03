namespace WinStudentGoalTracker.DataAccess;

public class dbStudentGoalRow
{
    public string? StudentIdentifier { get; set; }
    public required Guid GoalId { get; set; }
    public Guid? GoalParentId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int ProgressEventCount { get; set; }
}
