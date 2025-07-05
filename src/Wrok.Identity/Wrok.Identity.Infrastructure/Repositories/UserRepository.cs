
using Microsoft.EntityFrameworkCore;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;
using Wrok.Identity.Infrastructure.Data;

namespace Wrok.Identity.Infrastructure.Repositories;
internal sealed class UserRepository(WrokIdentityDbContext db) : IUserRepository
{
    public async Task<List<User>> GetAllAsync(CancellationToken ct)
    {
        return await db.Users.ToListAsync(ct);
    }

    public async Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken ct)
    {
        return await db.Users.Where(u => u.Role == role).ToListAsync(ct);
    }
}
