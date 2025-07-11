using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Tests.Entities;
[TestFixture]
internal class InvitationTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        var tenant = new Tenant("Tenant");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        tenant.AddAdminUser(admin);

        var invitation = new Invitation(admin, "invite@email.com", UserRole.ProjectManager, "code123");

        Assert.Multiple(() =>
        {
            Assert.That(invitation.Id, Is.Not.EqualTo(default(InvitationId)));
            Assert.That(invitation.InvitedToTenantId, Is.EqualTo(tenant.Id));
            Assert.That(invitation.InvitedByUserId, Is.EqualTo(admin.Id));
            Assert.That(invitation.Email, Is.EqualTo("invite@email.com"));
            Assert.That(invitation.Role, Is.EqualTo(UserRole.ProjectManager));
            Assert.That(invitation.Code, Is.EqualTo("code123"));
            Assert.That(invitation.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(invitation.AcceptedAt, Is.Null);
            Assert.That(invitation.Tenant, Is.EqualTo(tenant));
            Assert.That(invitation.InvitedByUser, Is.EqualTo(admin));
            Assert.That(invitation.CreatedUser, Is.Null);
            Assert.That(invitation.CreatedUserId, Is.Null);
        });
    }

    [Test]
    public void Constructor_ShouldThrow_WhenRoleIsInvalid()
    {
        var tenant = new Tenant("Tenant");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        tenant.AddAdminUser(admin);

        Assert.Throws<ArgumentException>(() =>
            new Invitation(admin, "invite@email.com", UserRole.Freelancer, "code123"));
    }

    [Test]
    public void AcceptBy_ShouldSetAcceptedAtAndCreatedUser()
    {
        var tenant = new Tenant("Tenant");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "pm@email.com", UserRole.ProjectManager, "code123");

        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        invitation.AcceptBy(pm);

        Assert.Multiple(() =>
        {
            Assert.That(invitation.AcceptedAt, Is.Not.Null);
            Assert.That(invitation.CreatedUser, Is.EqualTo(pm));
            Assert.That(invitation.CreatedUserId, Is.EqualTo(pm.Id));
        });
    }

    [Test]
    public void AcceptBy_ShouldThrow_IfAlreadyAccepted()
    {
        var tenant = new Tenant("Tenant");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "pm@email.com", UserRole.ProjectManager, "code123");

        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        invitation.AcceptBy(pm);

        var pm2 = new ProjectManagerUser("pm2@email.com", "PM2", "hash", "salt");
        Assert.Throws<InvalidOperationException>(() => invitation.AcceptBy(pm2));
    }

    [Test]
    public void AcceptBy_ShouldThrow_IfCreatedUserIsNull()
    {
        var tenant = new Tenant("Tenant");
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "pm@email.com", UserRole.ProjectManager, "code123");

        Assert.Throws<ArgumentNullException>(() => invitation.AcceptBy(null!));
    }
}
