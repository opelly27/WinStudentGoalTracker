namespace WinStudentGoalTracker.DataAccess;

public class dbStudent
{
    public required int IdStudent { get; set; }
    public int? IdProgram { get; set; }
    public string? Identifier { get; set; }
    public int? ProgramYear { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? ExpectedGrad { get; set; }
    public DateTime? CreatedAt { get; set; }
}
