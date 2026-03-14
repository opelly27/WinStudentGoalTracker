namespace WinStudentGoalTracker.DataAccess;

public class CreateGoalDto
{
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Baseline { get; set; }
    public Guid? GoalParentId { get; set; }
}
