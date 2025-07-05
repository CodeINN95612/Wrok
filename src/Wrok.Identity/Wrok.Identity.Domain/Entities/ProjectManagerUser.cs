using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;
public sealed class ProjectManagerUser : User
{
    public TenantId TenantId { get; private set; }
    public DateTime JoinedTenantAt { get; private set; }

    public Tenant Tenant { get; private set; }

#nullable disable
    // For EF Core
    private ProjectManagerUser() { }
#nullable enable

    public ProjectManagerUser(string email, string fullName, string passwordHash, string salt)
        : base(email, fullName, passwordHash, salt, UserRole.ProjectManager)
    {
        TenantId = new TenantId(Guid.Empty);
        Tenant = null!;
    }

    internal void JoinTenant(Tenant tenant)
    {
        if (tenant.Id.Value == Guid.Empty)
        {
            throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenant.Id));
        }
        TenantId = tenant.Id;
        Tenant = tenant;
        JoinedTenantAt = DateTime.UtcNow;
    }
}
