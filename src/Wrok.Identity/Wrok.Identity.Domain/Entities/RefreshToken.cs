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

#nullable disable
    private RefreshToken() { } //For EF Core
#nullable enable

    public RefreshToken(RefreshTokenId id, UserId userId, string token, DateTime expiration)
    {
        Id = id;
        UserId = userId;
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
    public bool IsExpired => DateTime.UtcNow >= Expiration;

}
