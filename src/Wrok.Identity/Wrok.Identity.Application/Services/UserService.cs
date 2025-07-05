
using ErrorOr;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.Services;
using Wrok.Identity.Application.Dtos.Users;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Services;
internal sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<ErrorOr<List<UserDto>>> GetAllUsersAsync(UserRole? role, CancellationToken cancellationToken)
    {
        List<User> users = role is null
            ? await userRepository.GetAllAsync(cancellationToken)
            : await userRepository.GetAllByRoleAsync(role.Value, cancellationToken);
        return users.Select(UserDto.FromUser).ToList();
    }
}
