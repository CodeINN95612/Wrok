using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Users.GetAllUsers;
internal static class GetAllUsersErrors
{
    public static ApplicationError NotAuthenticated =>
        new("GetAllUsers.NotAuthenticated", "Must be authenticated.");

    public static ApplicationError TenantNotFound =>
        new("GetAllUsers.TenantNotFound", "Tenant not found.");
}
