using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Data.Configurations;
internal sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .HasConversion(id => id.Value, value => new RefreshTokenId(value))
            .IsRequired();

        builder.Property(rt => rt.UserId)
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.Property(rt => rt.Expiration)
            .IsRequired();

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        builder.Property(rt => rt.RevokedAt)
            .IsRequired(false);

        builder.HasOne(rt => rt.User)
            .WithOne(u => u.RefreshToken)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
