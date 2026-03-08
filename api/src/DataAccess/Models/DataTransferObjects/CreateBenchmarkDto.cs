namespace WinStudentGoalTracker.DataAccess;

public class CreateBenchmarkDto
{
    public Guid GoalId { get; set; }
    public string Benchmark { get; set; } = string.Empty;
}
