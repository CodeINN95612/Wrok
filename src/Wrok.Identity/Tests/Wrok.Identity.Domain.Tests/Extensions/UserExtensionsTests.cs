using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;
using Wrok.Identity.Domain.Extensions;

namespace Wrok.Identity.Domain.Tests.Extensions;
[TestFixture]
internal class UserExtensionsTests
{
    [Test]
    public void GetTenant_ShouldReturnTenant_ForAdminUser()
    {
        var tenant = new Tenant("Tenant");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        admin.JoinTenant(tenant);

        var result = admin.GetTenant();

        Assert.That(result, Is.EqualTo(tenant));
    }

    [Test]
    public void GetTenant_ShouldReturnTenant_ForProjectManagerUser()
    {
        var tenant = new Tenant("Tenant");
        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        pm.JoinTenant(tenant);

        var result = pm.GetTenant();

        Assert.That(result, Is.EqualTo(tenant));
    }

    [Test]
    public void GetTenant_ShouldThrowArgumentNullException_IfAdminUserTenantIsNull()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        Assert.Throws<ArgumentNullException>(() => admin.GetTenant());
    }

    [Test]
    public void GetTenant_ShouldThrowArgumentNullException_IfProjectManagerUserTenantIsNull()
    {
        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        Assert.Throws<ArgumentNullException>(() => pm.GetTenant());
    }

    [Test]
    public void GetTenant_ShouldReturnNull_ForOtherUserTypes()
    {
        var user = new TestUser("other@email.com", "Other", "hash", "salt");
        var result = user.GetTenant();
        Assert.That(result, Is.Null);
    }

    private sealed class TestUser : User
    {
        public TestUser(string email, string fullName, string passwordHash, string salt)
            : base(email, fullName, passwordHash, salt, UserRole.Freelancer)
        {
        }
    }
}
