using Wrok.Identity.Application.Dtos.Tenants;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Dtos.Users;
public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    TenantDto? Tenant)
{
    public static UserDto FromUser(User user)
    {
        if (user is AdminUser admin)
        {
            return new UserDto(
                admin.Id.Value,
                admin.Email,
                admin.FullName,
                user.Role.ToString(),
                new(admin.Tenant.Id.Value, admin.Tenant.Name));
        }

        if (user is ProjectManagerUser pm)
        {
            return new UserDto(
                pm.Id.Value,
                pm.Email,
                pm.FullName,
                user.Role.ToString(),
                new(pm.Tenant.Id.Value, pm.Tenant.Name));
        }

        return new UserDto(
            user.Id.Value,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            null);
    }
};
