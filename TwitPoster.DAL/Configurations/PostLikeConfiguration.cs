using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Configurations;

internal sealed class PostLikeConfiguration :  IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(e => e.Post)
            .WithMany(p => p.PostLikes)
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasKey(e => new {e.UserId, e.PostId}).IsClustered();
    }
}