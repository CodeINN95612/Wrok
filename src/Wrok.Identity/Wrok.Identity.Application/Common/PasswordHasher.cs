using System.Security.Cryptography;

using Microsoft.Extensions.Configuration;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Common;
internal class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32;  // 256 bit
    private const int Iterations = 100_000;

    public (string passwordHash, string salt) Hash(string password)
    {
        
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Derive the key
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);

        // Combine salt and key, then encode as Base64
        var hashBytes = new byte[SaltSize + KeySize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(key, 0, hashBytes, SaltSize, KeySize);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(salt));
    }

    public string Hash(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        // Derive the key using the provided salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);

        // Combine salt and key, then encode as Base64
        var hashBytes = new byte[saltBytes.Length + KeySize];
        Array.Copy(saltBytes, 0, hashBytes, 0, saltBytes.Length);
        Array.Copy(key, 0, hashBytes, saltBytes.Length, KeySize);

        return Convert.ToBase64String(hashBytes);
    }
}
