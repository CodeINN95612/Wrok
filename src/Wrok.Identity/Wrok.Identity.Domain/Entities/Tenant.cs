namespace Wrok.Identity.Domain.Entities;

public sealed record TenantId(Guid Value);

public sealed class Tenant
{
    public TenantId Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<AdminUser> _adminUsers;
    public IReadOnlyCollection<AdminUser> AdminUsers => _adminUsers.AsReadOnly();

    private readonly List<ProjectManagerUser> _projectManagerUsers;
    public IReadOnlyCollection<ProjectManagerUser> ProjectManagerUsers => _projectManagerUsers.AsReadOnly();

    private readonly List<Invitation> _invitations = [];
    public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();

#nullable disable
    private Tenant() { } // For EF Core
#nullable enable

    public Tenant(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        Id = new TenantId(Guid.CreateVersion7());
        Name = name;
        CreatedAt = DateTime.UtcNow;
        _adminUsers = [];
        _projectManagerUsers = [];
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    public void AddAdminUser(AdminUser adminUser)
    {
        adminUser.JoinTenant(this);
        if (!_adminUsers.Any(u => u.IsSuper) && !adminUser.IsSuper)
        {
            throw new InvalidOperationException("At least one super admin user must be added to the tenant.");
        }
        if (_adminUsers.Any(u => u.IsSuper) && adminUser.IsSuper) 
        {
            throw new InvalidOperationException("Only one super admin user is allowed in the tenant.");
        }
        _adminUsers.Add(adminUser);
    }

    public void AddProjectManagerUser(ProjectManagerUser projectManagerUser)
    {
        projectManagerUser.JoinTenant(this);
        _projectManagerUsers.Add(projectManagerUser);
    }

    public AdminUser? GetAdminUser(UserId userId)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        var user = _adminUsers.FirstOrDefault(u => u.Id == userId);
        return user;
    }

    public User? GetUser(UserId userId)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        User? user = _adminUsers.FirstOrDefault(u => u.Id == userId);
        user ??= _projectManagerUsers.FirstOrDefault(u => u.Id == userId);

        return user;
    }

    public void Invite(Invitation invitation)
    {
        ArgumentNullException.ThrowIfNull(invitation, nameof(invitation));
        if (_invitations.Any(i => i.Email == invitation.Email))
        {
            throw new InvalidOperationException("An invitation with the same email already exists.");
        }
        _invitations.Add(invitation);
    }

    public void JoinByInvite(User user, string inviteCode)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentException.ThrowIfNullOrWhiteSpace(inviteCode, nameof(inviteCode));
        
        var invitation = _invitations.FirstOrDefault(i => i.Code == inviteCode);
        if (invitation is null)
        {
            throw new InvalidOperationException("Invitation not found or invalid.");
        }

        if (user is AdminUser adminUser)
        {
            AddAdminUser(adminUser);
        }
        else if (user is ProjectManagerUser projectManagerUser)
        {
            AddProjectManagerUser(projectManagerUser);
        }
        else
        {
            throw new InvalidOperationException("User type is not supported for tenant joining.");
        }

        invitation.AcceptBy(user);
    }
}
