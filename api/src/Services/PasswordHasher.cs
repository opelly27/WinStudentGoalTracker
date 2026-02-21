using System.Security.Cryptography;
using System.Text;

namespace WinStudentGoalTracker.Services;

public class PasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int HashSize = 32; // 256 bit
    private const int Iterations = 100_000;

    public static (string Hash, string Salt) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] saltBytes = new byte[SaltSize];
        rng.GetBytes(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(HashSize);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(HashSize);

        string incomingHash = Convert.ToBase64String(hashBytes);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(incomingHash),
            Encoding.UTF8.GetBytes(storedHash));
    }
}
