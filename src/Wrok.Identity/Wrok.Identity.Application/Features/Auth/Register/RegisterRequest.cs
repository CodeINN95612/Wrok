
using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Auth.Register;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    string TenantName) : IRequest<ErrorOr<RegisterResponse>>;
