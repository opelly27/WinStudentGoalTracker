namespace WinStudentGoalTracker.DataAccess;

public class dbRefreshToken
{
    public int IdRefreshToken { get; set; }
    public int IdUser { get; set; }
    public required string TokenHash { get; set; }
    public required string TokenSalt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime LastUsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? DeviceInfo { get; set; }
    public string? UserAgent { get; set; }
    public int? ReplacedByTokenId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
