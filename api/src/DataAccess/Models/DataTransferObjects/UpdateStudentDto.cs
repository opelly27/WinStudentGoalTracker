namespace WinStudentGoalTracker.DataAccess;

public class UpdateStudentDto
{
    public Guid? IdProgram { get; set; }
    public string? Identifier { get; set; }
    public int? ProgramYear { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? ExpectedGrad { get; set; }
}
