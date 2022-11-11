using Microsoft.EntityFrameworkCore;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL;

public sealed class TwitPosterContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();

    public TwitPosterContext (DbContextOptions<TwitPosterContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
            {
                builder.Property(user => user.Email).HasMaxLength(1000);
                builder.HasIndex(user => user.Email).IsUnique();
                builder.HasOne(user => user.UserAccount)
                    .WithOne()
                    .HasForeignKey<UserAccount>(r => r.UserId);
                
                builder.Property(user => user.FirstName).HasMaxLength(300);
                builder.Property(user => user.LastName).HasMaxLength(300);
            });

        modelBuilder.Entity<UserSubscription>(builder =>
        {
            builder.HasOne(e => e.Subscriber).WithMany()
                .HasForeignKey(e => e.SubscriberId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(e => e.Subscription).WithMany()
                .HasForeignKey(e => e.SubscriptionId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasKey(e => new {e.SubscriberId, e.SubscriptionId});
        });
            
        
        modelBuilder.Entity<UserAccount>(builder =>
        {
            builder.Property(userAccount => userAccount.Role)
                .HasConversion<string>()
                .HasMaxLength(100);
            builder.Property(userAccount => userAccount.Password).HasMaxLength(50);
        });

        modelBuilder.Entity<PostComment>(builder =>
        {
            builder.HasOne(e => e.Author)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Post)
                .WithMany(e => e.Comments)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(c => c.Text)
                .HasMaxLength(1000);
        });
        
        modelBuilder.Entity<PostLike>(builder =>
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
        });
    }
}