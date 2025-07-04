using Microsoft.EntityFrameworkCore;

using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Data;
internal sealed class WrokIdentityDbContext : DbContext
{
    public WrokIdentityDbContext(DbContextOptions<WrokIdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
