
using ErrorOr;

using MediatR;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Dtos.Users;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Users.GetById;

public sealed record GetUserByIdRequest(Guid UserId) : IRequest<ErrorOr<GetUserByIdResponse>>;
public sealed record GetUserByIdResponse(UserDto User);

internal sealed class GetUserByIdCommandHandler(
    IUserRepository userRepository) : IRequestHandler<GetUserByIdRequest, ErrorOr<GetUserByIdResponse>>
{
    public async Task<ErrorOr<GetUserByIdResponse>> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var id = new UserId(request.UserId);
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return Error.NotFound(
                code: "GetUserById.UserNotFound",
                description: $"User with ID {request.UserId} not found.");
        }

        var userDto = UserDto.FromUser(user);
        return new GetUserByIdResponse(userDto);
    }
}
