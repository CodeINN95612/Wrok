using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;
public sealed class AdminUser : User
{
    public TenantId TenantId { get; private set; }
    public DateTime JoinedTenantAt { get; private set; }
    public bool IsSuper { get; private set; } //TODO: Using this boolean might not follow DDD, more investigation needed

    public Tenant Tenant { get; private set; }

#nullable disable
    private AdminUser() { } // For EF Core
#nullable enable

    public AdminUser(string email, string fullName, string passwordHash, string salt, bool isSuper = false)
        : base(email, fullName, passwordHash, salt, UserRole.Admin)
    {
        TenantId = new TenantId(Guid.Empty);
        IsSuper = isSuper;
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
