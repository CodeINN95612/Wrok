using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Domain.Extensions;

public static class UserExtensions
{
    public static Tenant? GetTenant(this User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        if (user is AdminUser a)
        {
            ArgumentNullException.ThrowIfNull(a.Tenant, nameof(a.Tenant));
            return a.Tenant;
        }
        if (user is ProjectManagerUser pm)
        {
            ArgumentNullException.ThrowIfNull(pm.Tenant, nameof(pm.Tenant));
            return pm.Tenant;
        }
        return null;
    }
}
