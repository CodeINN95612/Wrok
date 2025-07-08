
using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Dtos.Users;

namespace Wrok.Identity.Application.Features.Users.GetAllUsers;

internal sealed class GetAllUsersCommandHandler(IUserRepository userRepository) : IRequestHandler<GetAllUsersRequest, ErrorOr<GetAllUsersResponse>>
{
    public async Task<ErrorOr<GetAllUsersResponse>> Handle(GetAllUsersRequest request, CancellationToken ct)
    {
        var users = await userRepository.GetAllAsync(ct);
        var usersDto = users
            .Select(UserDto.FromUser)
            .ToList();
        var count = usersDto.Count;
        return new GetAllUsersResponse(count, usersDto);
    }
}