
using ErrorOr;

using MediatR;

namespace Wrok.Identity.Application.Features.Users.GetAllUsers;

public sealed record GetAllUsersRequest : IRequest<ErrorOr<GetAllUsersResponse>>;
