namespace WinStudentGoalTracker.DataAccess;

public class dbStudent
{
    public required Guid IdStudent { get; set; }
    public Guid? IdProgram { get; set; }
    public Guid PrimaryTeacherId { get; set; }
    public string? Identifier { get; set; }
    public int? ProgramYear { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? ExpectedGrad { get; set; }
    public DateTime? CreatedAt { get; set; }
}
