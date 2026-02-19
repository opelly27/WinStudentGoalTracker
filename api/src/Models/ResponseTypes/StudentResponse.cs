using WinStudentGoalTracker.DataAccess;

namespace WinStudentGoalTracker.Models;

public class StudentResponse
{
    public int IdStudent { get; set; }
    public int? IdProgram { get; set; }
    public string? Identifier { get; set; }
    public int? ProgramYear { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? ExpectedGrad { get; set; }
    public DateTime? CreatedAt { get; set; }

    public static StudentResponse FromDatabaseModel(dbStudent student)
    {
        return new StudentResponse
        {
            IdStudent = student.IdStudent,
            IdProgram = student.IdProgram,
            Identifier = student.Identifier,
            ProgramYear = student.ProgramYear,
            EnrollmentDate = student.EnrollmentDate,
            ExpectedGrad = student.ExpectedGrad,
            CreatedAt = student.CreatedAt
        };
    }
}
