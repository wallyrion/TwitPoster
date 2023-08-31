using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Configurations;

internal sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.Property(userAccount => userAccount.Role)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(userAccount => userAccount.Password).HasMaxLength(50);
    }
}