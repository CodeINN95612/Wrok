using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Tests.Entities;

[TestFixture]
public class ProjectManagerUserTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var email = "pm@example.com";
        var fullName = "Project Manager";
        var passwordHash = "pmhash";
        var salt = "pmsalt";

        // Act
        var projectManagerUser = new ProjectManagerUser(email, fullName, passwordHash, salt);

        Assert.Multiple(() =>
        {
            Assert.That(email, Is.EqualTo(projectManagerUser.Email));
            Assert.That(fullName, Is.EqualTo(projectManagerUser.FullName));
            Assert.That(passwordHash, Is.EqualTo(projectManagerUser.PasswordHash));
            Assert.That(salt, Is.EqualTo(projectManagerUser.Salt));
            Assert.That(projectManagerUser.Role, Is.EqualTo(UserRole.ProjectManager));
            Assert.That(projectManagerUser.TenantId, Is.EqualTo(new TenantId(Guid.Empty)));
            Assert.That(projectManagerUser.Tenant, Is.Null);
        });
    }

    [Test]
    public void JoinTenant_ShouldSetTenantAndJoinedTenantAt()
    {
        // Arrange
        var projectManagerUser = new ProjectManagerUser("pm@example.com", "Project Manager", "pmhash", "pmsalt");
        var tenant = new Tenant("TenantName");

        // Act
        projectManagerUser.JoinTenant(tenant);

        Assert.Multiple(() =>
        {
            Assert.That(projectManagerUser.TenantId, Is.EqualTo(tenant.Id));
            Assert.That(projectManagerUser.Tenant, Is.EqualTo(tenant));
            Assert.That(projectManagerUser.JoinedTenantAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        });
    }
}
