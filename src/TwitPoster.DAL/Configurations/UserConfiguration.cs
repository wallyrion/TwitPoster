using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.Email).HasMaxLength(1000);
        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasOne(user => user.UserAccount)
            .WithOne()
            .HasForeignKey<UserAccount>(r => r.UserId);

        builder.Property(user => user.FirstName).HasMaxLength(300);
        builder.Property(user => user.LastName).HasMaxLength(300);
    }
}