namespace WinStudentGoalTracker.DataAccess;

public class UpdateProgressEventDto
{
    public string? Content { get; set; }
    public List<Guid>? BenchmarkIds { get; set; }
}
