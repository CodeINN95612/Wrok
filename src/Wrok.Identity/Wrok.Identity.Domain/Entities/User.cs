using Wrok.Identity.Domain.Enums;
using Wrok.Identity.Domain.Policies;

namespace Wrok.Identity.Domain.Entities;

public sealed record UserId(Guid Value);

public abstract class User
{
    public UserId Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public RefreshToken? RefreshToken { get; private set; }

#nullable disable
    protected User() { } // For EF Core
#nullable enable

    protected User(string email, string fullName, string passwordHash, string salt, UserRole role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName, nameof(fullName));
        Id = new UserId(Guid.CreateVersion7());
        Email = email;
        FullName = fullName;
        PasswordHash = passwordHash;
        Salt = salt;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        Email = email;
    }

    public void UpdateFullName(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName, nameof(fullName));
        FullName = fullName;
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash, nameof(passwordHash));
        PasswordHash = passwordHash;
    }

    public void UpdateRefreshToken(string token, IRefreshTokenExpirationPolicy expirationPolicy)
    {
        var expiration = expirationPolicy.GetExpirationDate();

        if (RefreshToken is null)
        {
            RefreshToken = new RefreshToken(
                this,
                token,
                expiration);
            return;
        }

        RefreshToken.UpdateToken(token, expiration);
    }
}
