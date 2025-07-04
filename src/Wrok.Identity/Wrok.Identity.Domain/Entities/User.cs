using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;

public sealed record UserId(Guid Value);

public abstract class User
{
    public UserId Id { get; private set; }
    public string Email { get; private set; }
    public string FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

#nullable disable
    protected User() { } // For EF Core
#nullable enable

    protected User(string email, string fullName, string passwordHash, UserRole role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName, nameof(fullName));
        Id = new UserId(Guid.CreateVersion7());
        Email = email;
        FullName = fullName;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        Role = role;
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
}
