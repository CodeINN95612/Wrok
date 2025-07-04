using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;
public sealed class AdminUser : User
{
    public TenantId TenantId { get; private set; }
    public bool IsSuper { get; private set; } //TODO: Using this boolean might not follow DDD, more investigation needed

#nullable disable
    private AdminUser() { } // For EF Core
#nullable enable

    public AdminUser(string username, string email, string fullName, TenantId tenantId, bool isSuper = false)
        : base(username, email, fullName, UserRole.Admin)
    {
        ArgumentNullException.ThrowIfNull(tenantId, nameof(tenantId));
        TenantId = tenantId;
        IsSuper = isSuper;
    }
}
