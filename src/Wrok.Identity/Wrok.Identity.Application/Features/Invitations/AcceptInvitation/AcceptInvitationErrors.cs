using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Invitations.AcceptInvitation;
internal static class AcceptInvitationErrors
{
    public static ApplicationError CodeRequired =>
        CommonErrors.ResourceRequired("AcceptInvitation.Code", "Code");

    public static ApplicationError EmailRequired =>
        CommonErrors.ResourceRequired("AcceptInvitation.Email", "Email");

    public static ApplicationError EmailInvalid =>
        CommonErrors.ResourceInvalid("AcceptInvitation.Email", "Email");

    public static ApplicationError PasswordRequired =>
        CommonErrors.ResourceRequired("AcceptInvitation.Password", "Password");

    public static ApplicationError PasswordInvalid =>
        CommonErrors.PasswordInvalid("AcceptInvitation.Password");

    public static ApplicationError FullnameRequired =>
        CommonErrors.ResourceRequired("AcceptInvitation.Fullname", "Fullname");

    public static ApplicationError InvitationNotFound =>
        CommonErrors.ResourceNotFound("AcceptInvitation", "Invitation");

    public static ApplicationError InvitationAlreadyAccepted =>
        new ApplicationError("AcceptInvitation.AlreadyAccepted", "Invitation has already been accepted.");
}
