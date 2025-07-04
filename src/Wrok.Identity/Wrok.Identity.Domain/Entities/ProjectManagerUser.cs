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

    public ProjectManagerUser(string email, string fullName, string passwordHash, TenantId tenantId)
        : base(email, fullName, passwordHash, UserRole.ProjectManager)
    {
        ArgumentNullException.ThrowIfNull(tenantId, nameof(tenantId));
        TenantId = tenantId;
        JoinedTenantAt = DateTime.UtcNow;
    }
}
