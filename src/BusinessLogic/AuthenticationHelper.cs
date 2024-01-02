using System.Security.Cryptography;
using System.Text;

namespace theforum.BusinessLogic;

public static class AuthenticationHelper
{
    private const int SaltLength = 16;
    private const int HashLength = 48;
    private const int Pbkdf2Iterations = 1000;
    public static string GetPasswordSalt()
    {
        var saltBuffer = RandomNumberGenerator.GetBytes(SaltLength);
        return Convert.ToBase64String(saltBuffer);
    }

    public static string GetPasswordHash(string password, string passwordSalt)
    {
        var saltedPasswordBytes = Encoding.UTF8.GetBytes(passwordSalt);
        var passwordHashGenerator = new Rfc2898DeriveBytes(password, saltedPasswordBytes, Pbkdf2Iterations);
        var passwordHashBytes = passwordHashGenerator.GetBytes(HashLength);
        return Convert.ToBase64String(passwordHashBytes);
    }

    public static bool ValidatePassword(string userAccountPasswordSalt, string userAccountPasswordHash, string passwordToValidate)
    {
        var hashedPasswordToValidate = GetPasswordHash(passwordToValidate, userAccountPasswordSalt);
        return hashedPasswordToValidate == userAccountPasswordHash;
    }
}