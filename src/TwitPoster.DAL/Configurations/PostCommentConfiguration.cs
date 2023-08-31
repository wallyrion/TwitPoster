using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Configurations;

internal sealed class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.HasOne(e => e.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Post>()
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Text)
            .HasMaxLength(20000);
    }
}