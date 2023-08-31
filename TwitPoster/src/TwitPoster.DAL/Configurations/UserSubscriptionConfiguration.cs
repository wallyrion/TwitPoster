using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Configurations;

internal sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.HasOne(e => e.Subscriber).WithMany()
            .HasForeignKey(e => e.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Subscription).WithMany()
            .HasForeignKey(e => e.SubscriptionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasKey(e => new { e.SubscriberId, e.SubscriptionId });
    }
}