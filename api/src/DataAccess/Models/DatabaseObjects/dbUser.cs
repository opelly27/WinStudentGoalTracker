namespace WinStudentGoalTracker.DataAccess;

public class dbUser
{
    public required Guid IdUser { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PasswordHash { get; set; }
    public string? PasswordSalt { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }
    public DateTime? CreatedAt { get; set; }
}
