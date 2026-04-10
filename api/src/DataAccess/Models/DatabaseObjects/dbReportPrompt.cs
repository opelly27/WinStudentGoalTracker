namespace WinStudentGoalTracker.DataAccess;

public class dbReportPrompt
{
    public required Guid IdReportPrompt { get; set; }
    public Guid? IdProgram { get; set; }
    public string? Prompt { get; set; }
    public string? Reportname { get; set; }
}
