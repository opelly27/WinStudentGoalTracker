namespace WinStudentGoalTracker.DataAccess;

public class UpdateGoalDto
{
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Baseline { get; set; }
    public DateTime? TargetCompletionDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public bool? Achieved { get; set; }
    public string? CloseNotes { get; set; }
}
