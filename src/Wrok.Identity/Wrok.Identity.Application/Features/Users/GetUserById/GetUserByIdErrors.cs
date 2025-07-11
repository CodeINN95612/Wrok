using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Users.GetUserById;

internal static class GetUserByIdErrors
{
    public static ApplicationError NotAuthenticated =>
        new("GetUserById.NotAuthenticated", "You must be authenticated to perform this action.");

    public static ApplicationError TenantNotFound =>
        CommonErrors.ResourceNotFound("GetUserById.Tenant", "Tenant");

    public static ApplicationError UserNotFound =>
        CommonErrors.ResourceNotFound("GetUserById.User", "User");
}
