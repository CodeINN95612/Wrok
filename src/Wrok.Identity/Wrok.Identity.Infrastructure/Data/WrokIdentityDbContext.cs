using Microsoft.EntityFrameworkCore;

using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Infrastructure.Data.Configurations;

namespace Wrok.Identity.Infrastructure.Data;
internal sealed class WrokIdentityDbContext(DbContextOptions<WrokIdentityDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<AdminUser> AdminUsers { get; set; } = default!;
    public DbSet<FreelancerUser> FreelanceUsers { get; set; } = default!;
    public DbSet<ProjectManagerUser> ProjectManagerUsers { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AdminUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FreelancerUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectManagerUserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityConfiguration());
    }
}
