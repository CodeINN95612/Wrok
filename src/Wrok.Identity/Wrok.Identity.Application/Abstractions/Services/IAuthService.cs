using ErrorOr;

using Wrok.Identity.Application.Dtos.Auth;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Abstractions.Services;
public interface IAuthService
{
    public Task<ErrorOr<UserId>> RegisterUserAsync(RegisterUserDto registerDto, CancellationToken ct);
}
