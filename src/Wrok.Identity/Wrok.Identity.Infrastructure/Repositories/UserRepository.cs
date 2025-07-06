
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
            .ToListAsync(ct);

        var projectManagerUsers = await db.Users
            .OfType<ProjectManagerUser>()
            .Include(u => u.Tenant)
            .ToListAsync(ct);

        var freelanceUsers = await db.Users
            .OfType<FreelancerUser>()
            .ToListAsync(ct);

        return [..adminUsers, ..projectManagerUsers, ..freelanceUsers];
    }

    public async Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken ct)
    {
        if (role == UserRole.Admin)
        {
            var users = await db.Users.OfType<AdminUser>().Include(u => u.Tenant).ToListAsync(ct);
            return [..users];
        }

        if (role == UserRole.ProjectManager)
        {
            var users = await db.Users.OfType<ProjectManagerUser>().Include(u => u.Tenant).ToListAsync(ct);
            return [..users];
        }

        return await db.Users.Where(u => u.Role == role).ToListAsync(ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
        {
            return null;
        }

        if (user is AdminUser adminUser)
        {
            await db.Entry(adminUser).Reference(u => u.Tenant).LoadAsync(ct);
            return adminUser;
        }

        if (user is ProjectManagerUser projectManagerUser)
        {
            await db.Entry(projectManagerUser).Reference(u => u.Tenant).LoadAsync(ct);
            return projectManagerUser;
        }

        return user;
    }
}
