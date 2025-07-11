using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Auth.Login;
internal static class LoginErrors
{
    public static ApplicationError EmailRequired =>
        CommonErrors.ResourceRequired("Login.Email", "Email");
    public static ApplicationError EmailInvalid =>
        CommonErrors.ResourceInvalid("Login.Email", "Email");

    public static ApplicationError PasswordRequired =>
        CommonErrors.ResourceRequired("Login.Password", "Password");

    public static ApplicationError InvalidCredentials =>
        new("Login.InvalidCredentials", "Invalid email or password.");
}
