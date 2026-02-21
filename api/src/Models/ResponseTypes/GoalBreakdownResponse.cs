namespace WinStudentGoalTracker.Models.ResponseTypes;

public class GoalBreakdownResponse
{
    public string Goal { get; set; } = string.Empty;
    public List<string> Subgoals { get; set; } = [];
}
