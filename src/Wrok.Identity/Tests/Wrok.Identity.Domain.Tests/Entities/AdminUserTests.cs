using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Tests.Entities;

[TestFixture]
public class AdminUserTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var email = "admin@example.com";
        var fullName = "Admin User";
        var passwordHash = "hash";
        var salt = "salt";
        var isSuper = true;

        // Act
        var adminUser = new AdminUser(email, fullName, passwordHash, salt, isSuper);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(email, Is.EqualTo(adminUser.Email));
            Assert.That(fullName, Is.EqualTo(adminUser.FullName));
            Assert.That(passwordHash, Is.EqualTo(adminUser.PasswordHash));
            Assert.That(salt, Is.EqualTo(adminUser.Salt));
            Assert.That(adminUser.Role, Is.EqualTo(UserRole.Admin));
            Assert.That(adminUser.IsSuper, Is.EqualTo(isSuper));
            Assert.That(adminUser.TenantId, Is.EqualTo(new TenantId(Guid.Empty)));
            Assert.That(adminUser.Tenant, Is.Null);
        });
    }

    [Test]
    public void JoinTenant_ShouldSetTenantAndJoinedTenantAt()
    {
        // Arrange
        var adminUser = new AdminUser("admin@example.com", "Admin User", "hash", "salt");
        var tenant = new Tenant("TenantName");

        // Act
        adminUser.JoinTenant(tenant);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(adminUser.TenantId, Is.EqualTo(tenant.Id));
            Assert.That(adminUser.Tenant, Is.EqualTo(tenant));
            Assert.That(adminUser.JoinedTenantAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        });
    }
}