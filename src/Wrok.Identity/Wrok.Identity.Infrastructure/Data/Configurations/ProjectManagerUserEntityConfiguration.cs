using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Data.Configurations;
internal sealed class ProjectManagerUserEntityConfiguration : IEntityTypeConfiguration<ProjectManagerUser>
{
    public void Configure(EntityTypeBuilder<ProjectManagerUser> builder)
    {
        builder.HasBaseType<User>();

        builder.Property(pm => pm.TenantId)
            .HasConversion(
                v => v.Value,
                v => new TenantId(v))
            .IsRequired();

        builder.Property(pm => pm.JoinedTenantAt)
            .IsRequired();

        builder.HasOne(pm => pm.Tenant)
            .WithMany(t => t.ProjectManagerUsers)
            .HasForeignKey(pm => pm.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
