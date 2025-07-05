using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;
public sealed class ProjectManagerUser : User
{
    public TenantId TenantId { get; private set; }
    public DateTime JoinedTenantAt { get; private set; }

#nullable disable
    // For EF Core
    private ProjectManagerUser() { }
#nullable enable

    public ProjectManagerUser(string email, string fullName, string passwordHash, string salt)
        : base(email, fullName, passwordHash, salt, UserRole.ProjectManager)
    {
        TenantId = new TenantId(Guid.Empty);
    }

    internal void JoinTenant(TenantId tenantId)
    {
        if (tenantId.Value == Guid.Empty)
        {
            throw new ArgumentException("Tenant ID cannot be empty.", nameof(tenantId));
        }
        TenantId = tenantId;
        JoinedTenantAt = DateTime.UtcNow;
    }
}
