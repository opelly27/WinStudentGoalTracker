namespace WinStudentGoalTracker.Models;

public class StudentBenchmarkSummary
{
    public string? StudentIdentifier { get; set; }
    public List<StudentBenchmarkItem> Benchmarks { get; set; } = [];
}
