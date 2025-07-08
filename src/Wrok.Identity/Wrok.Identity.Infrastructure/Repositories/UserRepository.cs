

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
        var adminUsers = await db.AdminUsers
            .Include(u => u.Tenant)
            .Include(u => u.RefreshToken)
            .Cast<User>()
            .ToListAsync(ct);

        var projectManagerUsers = await db.ProjectManagerUsers
            .Include(u => u.Tenant)
            .Include(u => u.RefreshToken)
            .Cast<User>()
            .ToListAsync(ct);

        var freelanceUsers = await db.FreelancerUsers
            .Include(u => u.RefreshToken)
            .Cast<User>()
            .ToListAsync(ct);

        return adminUsers.Union(projectManagerUsers).Union(freelanceUsers).ToList();
    }

    public async Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken ct)
    {
        if (role == UserRole.Admin)
        {
            var users = await db.AdminUsers
                .Include(u => u.Tenant)
                .Include(u => u.RefreshToken)
                .Cast<User>()
                .ToListAsync(ct);
            return users;
        }

        if (role == UserRole.ProjectManager)
        {
            var users = await db.ProjectManagerUsers
                .Include(u => u.Tenant)
                .Include(u => u.RefreshToken)
                .Cast<User>()
                .ToListAsync(ct);
            return users;
        }

        var freeelancerUsers = await db.FreelancerUsers
            .Include(u => u.RefreshToken)
            .Cast<User>()
            .ToListAsync(ct);
        return freeelancerUsers;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
        {
            return null;
        }

        await LoadTenantAsync(user, ct);

        return user;
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null)
        {
            return null;
        }

        await LoadTenantAsync(user, ct);
        return user;
    }

    public async Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct)
    {
        var user = await db.Users
            .Include(u => u.RefreshToken)
            .FirstOrDefaultAsync(u => u.RefreshToken != null && u.RefreshToken.Token == token, ct);

        if (user is null)
        {
            return null;
        }

        await LoadTenantAsync(user, ct);

        return user;
    }

    public async Task<bool> IsUniqueByEmailAsync(string email, CancellationToken ct)
    {
        return await db.Users.AnyAsync(u => u.Email == email, ct);
    }

    public void Update(User user)
    {
        db.Update(user);
    }

    private async Task LoadTenantAsync(User user, CancellationToken ct)
    {
        if (user is AdminUser adminUser)
        {
            await db.Entry(adminUser).Reference(u => u.Tenant).LoadAsync(ct);
            return;
        }
        if (user is ProjectManagerUser projectManagerUser)
        {
            await db.Entry(projectManagerUser).Reference(u => u.Tenant).LoadAsync(ct);
            return;
        }
        return;
    }
}
