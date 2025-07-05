using ErrorOr;

using Wrok.Identity.Application.Dtos.Users;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Abstractions.Services;
public interface IUserService
{
    public Task<ErrorOr<List<UserDto>>> GetAllUsersAsync(UserRole? role, CancellationToken cancellationToken);
}
