using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Features.Auth.Login;

public sealed record LoginResponse(
    string Email,
    string FullName,
    string Role,
    string Token,
    string RefreshToken);
