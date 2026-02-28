namespace WinStudentGoalTracker.DataAccess;

public class dbUserStudent
{
    public required Guid IdUserStudent { get; set; }
    public Guid? IdUser { get; set; }
    public Guid? IdStudent { get; set; }
    public bool? IsPrimary { get; set; }
}
