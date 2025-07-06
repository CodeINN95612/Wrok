
using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Auth.RefreshTokens;

public sealed record RefreshTokenRequest(
    string Token) : IRequest<ErrorOr<RefreshTokenResponse>>;
