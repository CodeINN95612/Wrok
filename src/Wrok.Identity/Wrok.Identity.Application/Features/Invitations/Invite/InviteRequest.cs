
using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Invitations.Invite;

public record struct InviteRequest(string Email, string Role) : IRequest<ErrorOr<InviteResponse>>;
