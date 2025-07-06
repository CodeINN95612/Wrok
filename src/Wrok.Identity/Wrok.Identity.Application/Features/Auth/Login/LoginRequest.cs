
using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Auth.Login;

public sealed record LoginRequest(
    string Email,
    string Password) : IRequest<ErrorOr<LoginResponse>>;
