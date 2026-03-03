namespace WinStudentGoalTracker.Models;

public class StudentGoalSummary
{
    public string? StudentIdentifier { get; set; }
    public List<StudentGoalItem> Goals { get; set; } = [];
}
