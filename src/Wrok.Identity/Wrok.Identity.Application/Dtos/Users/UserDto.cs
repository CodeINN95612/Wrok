using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Dtos.Users;
public sealed record UserDto(
    UserId Id,
    string Email,
    string FullName,
    string Role,
    TenantId? TenantId)
{
    public static UserDto FromUser(User user)
    {
        if (user is AdminUser admin)
        {
            return new UserDto(
                admin.Id,
                admin.Email,
                admin.FullName,
                user.Role.ToString(),
                admin.TenantId);
        }

        if (user is ProjectManagerUser projectManager)
        {
            return new UserDto(
                projectManager.Id,
                projectManager.Email,
                projectManager.FullName,
                user.Role.ToString(),
                projectManager.TenantId);
        }

        return new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            null);
    }
};
