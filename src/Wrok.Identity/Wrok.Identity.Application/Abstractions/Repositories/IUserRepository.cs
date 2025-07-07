using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Abstractions.Repositories;
public interface IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken ct);
    public Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken ct);
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    public Task<bool> IsUniqueByEmailAsync(string email, CancellationToken ct);

    public Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct);

    public void Update(User user);
}
