using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Infrastructure.Data.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("Invitations");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, value => new InvitationId(value))
            .IsRequired();

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(i => i.Role)
            .IsRequired()
            .HasConversion(
                role => role.ToString(),
                value => Enum.Parse<UserRole>(value));

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasOne(i => i.Tenant)
            .WithMany(t => t.Invitations)
            .HasForeignKey(i => i.InvitedToTenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.Email, i.InvitedToTenantId })
            .IsUnique();

        builder.HasOne(i => i.InvitedByUser)
            .WithMany()
            .HasForeignKey(i => i.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.CreatedUser)
            .WithMany()
            .HasForeignKey(i => i.CreatedUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(i => i.CreatedUserId)
            .IsUnique();
    }
}
