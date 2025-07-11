using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Tests.Entities;
[TestFixture]
internal class TenantTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        var tenant = new Tenant("Acme");

        Assert.Multiple(() =>
        {
            Assert.That(tenant.Name, Is.EqualTo("Acme"));
            Assert.That(tenant.Id, Is.Not.Null);
            Assert.That(tenant.CreatedAt, Is.Not.EqualTo(default(DateTime)));
            Assert.That(tenant.AdminUsers, Is.Empty);
            Assert.That(tenant.ProjectManagerUsers, Is.Empty);
            Assert.That(tenant.Invitations, Is.Empty);
        });
    }

    [Test]
    public void UpdateName_ShouldChangeName()
    {
        var tenant = new Tenant("OldName");
        tenant.UpdateName("NewName");

        Assert.That(tenant.Name, Is.EqualTo("NewName"));
    }

    [Test]
    public void UpdateName_ShouldThrow_WhenNameIsNullOrWhitespace()
    {
        var tenant = new Tenant("Name");
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentException>(() => tenant.UpdateName(""));
            Assert.Throws<ArgumentException>(() => tenant.UpdateName(" "));
        });
    }

    [Test]
    public void AddAdminUser_ShouldAddSuperAdmin()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);

        Assert.Multiple(() =>
        {
            Assert.That(tenant.AdminUsers, Has.Count.EqualTo(1));
            Assert.That(tenant.AdminUsers.First().IsSuper, Is.True);
        });
    }

    [Test]
    public void AddAdminUser_ShouldThrow_IfNoSuperAdmin()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", false);
        var tenant = new Tenant("Tenant");
        Assert.Throws<InvalidOperationException>(() => tenant.AddAdminUser(admin));
    }

    [Test]
    public void AddAdminUser_ShouldThrow_IfMultipleSuperAdmins()
    {
        var superAdmin1 = new AdminUser("admin1@email.com", "Admin1", "hash", "salt", true);
        var superAdmin2 = new AdminUser("admin2@email.com", "Admin2", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(superAdmin1);
        Assert.Throws<InvalidOperationException>(() => tenant.AddAdminUser(superAdmin2));
    }

    [Test]
    public void AddProjectManagerUser_ShouldAddUser()
    {
        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        var tenant = new Tenant("Tenant");
        tenant.AddProjectManagerUser(pm);

        Assert.That(tenant.ProjectManagerUsers, Has.Count.EqualTo(1));
    }

    [Test]
    public void GetAdminUser_ShouldReturnCorrectUser()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);

        var found = tenant.GetAdminUser(admin.Id);
        Assert.That(found, Is.EqualTo(admin));
    }

    [Test]
    public void GetAdminUser_ShouldReturnNull_IfNotFound()
    {
        var tenant = new Tenant("Tenant");
        var userId = new UserId(Guid.NewGuid());
        var found = tenant.GetAdminUser(userId);
        Assert.That(found, Is.Null);
    }

    [Test]
    public void GetUser_ShouldReturnAdminOrProjectManager()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        tenant.AddProjectManagerUser(pm);

        Assert.Multiple(() =>
        {
            Assert.That(tenant.GetUser(admin.Id), Is.EqualTo(admin));
            Assert.That(tenant.GetUser(pm.Id), Is.EqualTo(pm));
        });
    }

    [Test]
    public void Invite_ShouldAddInvitation()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "invite@email.com", UserRole.ProjectManager, "code123");

        tenant.Invite(invitation);

        Assert.Multiple(() =>
        {
            Assert.That(tenant.Invitations, Has.Count.EqualTo(1));
            Assert.That(tenant.Invitations.First().Email, Is.EqualTo("invite@email.com"));
        });
    }

    [Test]
    public void Invite_ShouldThrow_IfEmailAlreadyInvited()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        var invitation1 = new Invitation(admin, "invite@email.com", UserRole.ProjectManager, "code123");
        var invitation2 = new Invitation(admin, "invite@email.com", UserRole.ProjectManager, "code456");

        tenant.Invite(invitation1);
        Assert.Throws<InvalidOperationException>(() => tenant.Invite(invitation2));
    }

    [Test]
    public void JoinByInvite_ShouldAddUserAndAcceptInvitation()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);
        var invitation = new Invitation(admin, "pm@email.com", UserRole.ProjectManager, "code123");
        tenant.Invite(invitation);

        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        tenant.JoinByInvite(pm, "code123");

        Assert.Multiple(() =>
        {
            Assert.That(tenant.ProjectManagerUsers, Has.Count.EqualTo(1));
            Assert.That(invitation.AcceptedAt, Is.Not.Null);
            Assert.That(invitation.CreatedUser, Is.EqualTo(pm));
        });
    }

    [Test]
    public void JoinByInvite_ShouldThrow_IfInvitationNotFound()
    {
        var admin = new AdminUser("admin@email.com", "Admin", "hash", "salt", true);
        var tenant = new Tenant("Tenant");
        tenant.AddAdminUser(admin);

        var pm = new ProjectManagerUser("pm@email.com", "PM", "hash", "salt");
        Assert.Throws<InvalidOperationException>(() => tenant.JoinByInvite(pm, "invalid_code"));
    }
}
