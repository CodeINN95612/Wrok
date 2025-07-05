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

    public void RemoveAdminUser(AdminUser adminUser)
    {
        var existingUser = _adminUsers.FirstOrDefault(u => u.Id == adminUser.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException("Admin user not found in the tenant.");
        }
        if (existingUser.IsSuper)
        {
            throw new InvalidOperationException("Cannot remove the super admin user from the tenant.");
        }
        _adminUsers.Remove(adminUser);
    }
}
