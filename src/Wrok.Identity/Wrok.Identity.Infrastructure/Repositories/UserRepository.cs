
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
        var adminUsers = await db.Users
            .OfType<AdminUser>()
            .Include(u => u.Tenant)
            .Include(u => u.RefreshToken)
            .ToListAsync(ct);

        var projectManagerUsers = await db.Users
            .OfType<ProjectManagerUser>()
            .Include(u => u.Tenant)
            .Include(u => u.RefreshToken)
            .ToListAsync(ct);

        var freelanceUsers = await db.Users
            .OfType<FreelancerUser>()
            .ToListAsync(ct);

        return [.. adminUsers, .. projectManagerUsers, .. freelanceUsers];
    }

    public async Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken ct)
    {
        if (role == UserRole.Admin)
        {
            var users = await db.Users
                .OfType<AdminUser>()
                .Include(u => u.Tenant)
                .Include(u => u.RefreshToken)
                .ToListAsync(ct);
            return [.. users];
        }

        if (role == UserRole.ProjectManager)
        {
            var users = await db.Users
                .OfType<ProjectManagerUser>()
                .Include(u => u.Tenant)
                .Include(u => u.RefreshToken)
                .ToListAsync(ct);
            return [.. users];
        }

        return await db.Users.Where(u => u.Role == role).ToListAsync(ct);
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
        await Task.CompletedTask;
        return;
    }
}
