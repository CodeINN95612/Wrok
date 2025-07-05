using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Data.Configurations;
internal class AdminUserEntityConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.HasBaseType<User>();

        builder.Property(a => a.TenantId)
            .HasConversion(
                v => v.Value,
                v => new TenantId(v))
            .IsRequired();

        builder.Property(a => a.JoinedTenantAt)
            .IsRequired();

        builder.Property(a => a.IsSuper)
            .IsRequired();

        builder.HasOne(a => a.Tenant)
            .WithMany(t => t.AdminUsers)
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
