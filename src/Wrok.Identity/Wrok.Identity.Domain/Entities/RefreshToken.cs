namespace Wrok.Identity.Domain.Entities;

public sealed record RefreshTokenId(Guid Value);

public sealed class RefreshToken
{
    public RefreshTokenId Id { get; private set; }
    public UserId UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime Expiration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public User User { get; private set; }

#nullable disable
    private RefreshToken() { } //For EF Core
#nullable enable

    internal RefreshToken(User user, string token, DateTime expiration)
    {
        Id = new(Guid.CreateVersion7());
        UserId = user.Id;
        User = user;
        Token = token;
        Expiration = expiration;
        CreatedAt = DateTime.UtcNow;
        RevokedAt = null;
    }
    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow > Expiration;

    internal void UpdateToken(string token, DateTime expiration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token, nameof(token));
        if (expiration <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiration must be in the future.", nameof(expiration));
        }
        Token = token;
        Expiration = expiration;
    }

}
