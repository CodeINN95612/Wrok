
using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Users.GetById;

public sealed record GetUserByIdRequest(Guid UserId) : IRequest<ErrorOr<GetUserByIdResponse>>;
