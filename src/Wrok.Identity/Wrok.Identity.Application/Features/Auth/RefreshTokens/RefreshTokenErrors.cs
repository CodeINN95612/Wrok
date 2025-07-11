using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Features.Auth.RefreshTokens;
internal static class RefreshTokenErrors
{
    public static ApplicationError RefreshTokenRequired => 
        CommonErrors.ResourceRequired("RefreshToken.Token", "Token");
    public static ApplicationError RefreshTokenUserNotFound =>
        CommonErrors.ResourceNotFound("RefreshToken.User", "RefreshToken User");
    public static ApplicationError RefreshTokenRevoked =>
        new("RefreshToken.Revoked", "The refresh token has been revoked.");
    public static ApplicationError RefreshTokenExpired =>
        new("RefreshToken.Expired", "The refresh token has expired.");
}
