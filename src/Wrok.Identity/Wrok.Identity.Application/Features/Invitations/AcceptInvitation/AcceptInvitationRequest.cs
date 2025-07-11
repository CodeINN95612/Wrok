using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Invitations.AcceptInvitation;

public sealed record AcceptInvitationRequest(
    string Code,
    string Email,
    string Password,
    string Fullname) : IRequest<ErrorOr<AcceptInvitationResponse>>;
