using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Data.Configurations;
internal sealed class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => new TenantId(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasMany(t => t.AdminUsers)
            .WithOne()
            .HasForeignKey(au => au.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.ProjectManagerUsers)
            .WithOne()
            .HasForeignKey(pmu => pmu.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}