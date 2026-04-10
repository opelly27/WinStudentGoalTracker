namespace WinStudentGoalTracker.Models;

public class ReportPromptResponse
{
    public Guid ReportPromptId { get; set; }
    public Guid? ProgramId { get; set; }
    public string? Prompt { get; set; }
    public string? Reportname { get; set; }
}
