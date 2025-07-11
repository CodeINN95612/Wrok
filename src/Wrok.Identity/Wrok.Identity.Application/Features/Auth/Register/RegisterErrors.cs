using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Auth.Register;
internal static class RegisterErrors
{
    public static ApplicationError EmailRequired =>
        CommonErrors.ResourceRequired("Register.Email", "Email");
    public static ApplicationError EmailInvalid =>
        CommonErrors.ResourceInvalid("Register.Email", "Email");
    public static ApplicationError PasswordRequired =>
        CommonErrors.ResourceRequired("Register.Password", "Password");
    public static ApplicationError PasswordInvalid =>
        CommonErrors.PasswordInvalid("Register.Password");
    public static ApplicationError FullNameRequired =>
        CommonErrors.ResourceRequired("Register.FullName", "Full Name");
    public static ApplicationError TenantNameRequired =>
        CommonErrors.ResourceRequired("Register.TenantName", "Tenant Name");
    
    public static ApplicationError EmailAlreadyInUse =>
        new("Register.Email.AlreadyInUse", "Email is already in use.");
}
