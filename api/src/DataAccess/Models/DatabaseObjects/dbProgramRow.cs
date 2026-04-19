namespace WinStudentGoalTracker.DataAccess;

public class dbProgramRow
{
    public Guid IdProgram { get; set; }
    public Guid IdSchoolDistrict { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
