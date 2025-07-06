using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Abstractions.Repositories;
public interface IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken ct);
    public Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);

    public void Update(User user);
}
