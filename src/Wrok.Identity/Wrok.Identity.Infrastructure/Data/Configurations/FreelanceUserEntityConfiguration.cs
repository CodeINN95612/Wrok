using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Infrastructure.Data.Configurations;
internal sealed class FreelancerUserEntityConfiguration : IEntityTypeConfiguration<FreelancerUser>
{
    public void Configure(EntityTypeBuilder<FreelancerUser> builder)
    {
        builder.HasBaseType<User>();

        builder.Property(f => f.Title)
            .IsRequired();

        builder.Property(f => f.Bio)
            .IsRequired();
    }
}
