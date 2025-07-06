namespace Wrok.Identity.Application.Features.Auth.RefreshTokens;

public sealed record RefreshTokenResponse(
    string JwtToken,
    string RefreshToken
);
