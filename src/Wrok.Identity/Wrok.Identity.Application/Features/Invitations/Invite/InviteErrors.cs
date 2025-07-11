using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Invitations.Invite;
internal static class InviteErrors
{
    public static ApplicationError EmailRequired =>
        CommonErrors.ResourceRequired("Invite.Email", "Email");

    public static ApplicationError InvalidEmail =>
        CommonErrors.ResourceInvalid("Invite.Email", "Email");

    public static ApplicationError RoleRequired =>
        CommonErrors.ResourceRequired("Invite.Role", "Role");

    public static ApplicationError InvalidRole =>
        CommonErrors.ResourceInvalid("Invite.Role", "Role");

    public static ApplicationError TenantNotFound =>
        CommonErrors.ResourceNotFound("Invite.Tenant", "Tenant");

    public static ApplicationError UserNotFound =>
        CommonErrors.ResourceNotFound("Invite.User", "User");
}
