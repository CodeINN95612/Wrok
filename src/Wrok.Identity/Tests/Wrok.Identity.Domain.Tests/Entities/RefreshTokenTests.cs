using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;
using Wrok.Identity.Domain.Policies;

namespace Wrok.Identity.Domain.Tests.Entities;
[TestFixture]
internal class RefreshTokenTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        var user = new AdminUser("user@email.com", "User", "hash", "salt");
        var expiration = DateTime.UtcNow.AddHours(1);
        var token = new RefreshToken(user, "token123", expiration);

        Assert.Multiple(() =>
        {
            Assert.That(token.Id, Is.Not.Null);
            Assert.That(token.UserId, Is.EqualTo(user.Id));
            Assert.That(token.User, Is.EqualTo(user));
            Assert.That(token.Token, Is.EqualTo("token123"));
            Assert.That(token.Expiration, Is.EqualTo(expiration));
            Assert.That(token.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(token.RevokedAt, Is.Null);
            Assert.That(token.IsRevoked, Is.False);
            Assert.That(token.IsExpired, Is.False);
        });
    }

    [Test]
    public void Revoke_ShouldSetRevokedAt()
    {
        var user = new AdminUser("user@email.com", "User", "hash", "salt");
        var token = new RefreshToken(user, "token123", DateTime.UtcNow.AddHours(1));
        token.Revoke();

        Assert.Multiple(() =>
        {
            Assert.That(token.RevokedAt, Is.Not.Null);
            Assert.That(token.IsRevoked, Is.True);
        });
    }

    [Test]
    public void IsExpired_ShouldReturnTrue_IfPastExpiration()
    {
        var user = new AdminUser("user@email.com", "User", "hash", "salt");
        var token = new RefreshToken(user, "token123", DateTime.UtcNow.AddSeconds(-1));
        Assert.That(token.IsExpired, Is.True);
    }

    [Test]
    public void UpdateToken_ShouldUpdateTokenAndExpiration_AndResetRevokedAt()
    {
        var user = new AdminUser("user@email.com", "User", "hash", "salt");
        var token = new RefreshToken(user, "oldtoken", DateTime.UtcNow.AddHours(1));
        token.Revoke();

        var newExpiration = DateTime.UtcNow.AddHours(2);
        token.UpdateToken("newtoken", newExpiration);

        Assert.Multiple(() =>
        {
            Assert.That(token.Token, Is.EqualTo("newtoken"));
            Assert.That(token.Expiration, Is.EqualTo(newExpiration));
            Assert.That(token.RevokedAt, Is.Null);
        });
    }

    [Test]
    public void UpdateToken_ShouldThrow_IfTokenIsNullOrWhitespace()
    {
        var user = new AdminUser("user@email.com", "User", "hash", "salt");
        var token = new RefreshToken(user, "token123", DateTime.UtcNow.AddHours(1));
        var future = DateTime.UtcNow.AddHours(2);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => token.UpdateToken("", future));
            Assert.Throws<ArgumentException>(() => token.UpdateToken(" ", future));
        });
    }

    [Test]
    public void UpdateToken_ShouldThrow_IfExpirationIsNotInFuture()
    {
        var user = new AdminUser("user@email.com", "User", "hash", "salt");
        var token = new RefreshToken(user, "token123", DateTime.UtcNow.AddHours(1));
        var past = DateTime.UtcNow.AddSeconds(-1);

        Assert.Throws<ArgumentException>(() => token.UpdateToken("newtoken", past));
    }
}
