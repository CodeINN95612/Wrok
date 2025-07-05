using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;
public sealed class AdminUser : User
{
    public TenantId TenantId { get; private set; }
    public DateTime JoinedTenantAt { get; private set; }
    public bool IsSuper { get; private set; } //TODO: Using this boolean might not follow DDD, more investigation needed

#nullable disable
    private AdminUser() { } // For EF Core
#nullable enable

    public AdminUser(string email, string fullName, string passwordHash, string salt, bool isSuper = false)
        : base(email, fullName, passwordHash, salt, UserRole.Admin)
    {
        TenantId = new TenantId(Guid.Empty);
        IsSuper = isSuper;
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
