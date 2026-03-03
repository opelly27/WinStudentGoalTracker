namespace WinStudentGoalTracker.DataAccess;

public class AddProgressEventDto
{
    public Guid GoalId { get; set; }
    public string? Content { get; set; }
    public bool IsSensitive { get; set; }
}
