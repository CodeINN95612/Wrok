using Moq;

using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;
using Wrok.Identity.Domain.Policies;

namespace Wrok.Identity.Domain.Tests.Entities;
[TestFixture]
internal class UserTests
{
    private class TestUser : User
    {
        public TestUser(string email, string fullName, string passwordHash, string salt, UserRole role)
            : base(email, fullName, passwordHash, salt, role) { }
    }

    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        var user = new TestUser("user@email.com", "User Name", "hash", "salt", UserRole.Admin);

        Assert.Multiple(() =>
        {
            Assert.That(user.Email, Is.EqualTo("user@email.com"));
            Assert.That(user.FullName, Is.EqualTo("User Name"));
            Assert.That(user.PasswordHash, Is.EqualTo("hash"));
            Assert.That(user.Salt, Is.EqualTo("salt"));
            Assert.That(user.Role, Is.EqualTo(UserRole.Admin));
            Assert.That(user.Id, Is.Not.Null);
            Assert.That(user.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(user.RefreshToken, Is.Null);
        });
    }

    [Test]
    public void UpdateEmail_ShouldChangeEmail()
    {
        var user = new TestUser("old@email.com", "User Name", "hash", "salt", UserRole.Admin);
        user.UpdateEmail("new@email.com");

        Assert.That(user.Email, Is.EqualTo("new@email.com"));
    }

    [Test]
    public void UpdateEmail_ShouldThrow_WhenEmailIsNullOrWhitespace()
    {
        var user = new TestUser("user@email.com", "User Name", "hash", "salt", UserRole.Admin);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => user.UpdateEmail(""));
            Assert.Throws<ArgumentException>(() => user.UpdateEmail(" "));
        });
    }

    [Test]
    public void UpdateFullName_ShouldChangeFullName()
    {
        var user = new TestUser("user@email.com", "Old Name", "hash", "salt", UserRole.Admin);
        user.UpdateFullName("New Name");

        Assert.That(user.FullName, Is.EqualTo("New Name"));
    }

    [Test]
    public void UpdateFullName_ShouldThrow_WhenFullNameIsNullOrWhitespace()
    {
        var user = new TestUser("user@email.com", "User Name", "hash", "salt", UserRole.Admin);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => user.UpdateFullName(""));
            Assert.Throws<ArgumentException>(() => user.UpdateFullName(" "));
        });
    }

    [Test]
    public void UpdatePasswordHash_ShouldChangePasswordHash()
    {
        var user = new TestUser("user@email.com", "User Name", "oldhash", "salt", UserRole.Admin);
        user.UpdatePasswordHash("newhash");

        Assert.That(user.PasswordHash, Is.EqualTo("newhash"));
    }

    [Test]
    public void UpdatePasswordHash_ShouldThrow_WhenPasswordHashIsNullOrWhitespace()
    {
        var user = new TestUser("user@email.com", "User Name", "hash", "salt", UserRole.Admin);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => user.UpdatePasswordHash(""));
            Assert.Throws<ArgumentException>(() => user.UpdatePasswordHash(" "));
        });
    }

    [Test]
    public void UpdateRefreshToken_ShouldSetRefreshToken_WhenNull()
    {
        var user = new TestUser("user@email.com", "User Name", "hash", "salt", UserRole.Admin);
        var mockPolicy = new Mock<IRefreshTokenExpirationPolicy>();
        var expiration = DateTime.UtcNow.AddDays(1);
        mockPolicy.Setup(p => p.GetExpirationDate()).Returns(expiration);

        user.UpdateRefreshToken("token123", mockPolicy.Object);

        Assert.Multiple(() =>
        {
            Assert.That(user.RefreshToken, Is.Not.Null);
            Assert.That(user.RefreshToken!.Token, Is.EqualTo("token123"));
            Assert.That(user.RefreshToken.Expiration, Is.EqualTo(expiration));
        });
    }

    [Test]
    public void UpdateRefreshToken_ShouldUpdateExistingToken()
    {
        var user = new TestUser("user@email.com", "User Name", "hash", "salt", UserRole.Admin);
        var mockPolicy = new Mock<IRefreshTokenExpirationPolicy>();
        var expiration1 = DateTime.UtcNow.AddDays(1);
        var expiration2 = DateTime.UtcNow.AddDays(2);
        mockPolicy.SetupSequence(p => p.GetExpirationDate())
            .Returns(expiration1)
            .Returns(expiration2);

        user.UpdateRefreshToken("token1", mockPolicy.Object);
        user.UpdateRefreshToken("token2", mockPolicy.Object);

        Assert.Multiple(() =>
        {
            Assert.That(user.RefreshToken, Is.Not.Null);
            Assert.That(user.RefreshToken!.Token, Is.EqualTo("token2"));
            Assert.That(user.RefreshToken.Expiration, Is.EqualTo(expiration2));
        });
    }
}
