using Microsoft.EntityFrameworkCore;
using TwitPoster.DAL.Configurations;
using TwitPoster.DAL.Models;
using TwitPoster.DAL.Seeding;

namespace TwitPoster.DAL;

public sealed class TwitPosterContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();

    public TwitPosterContext (DbContextOptions<TwitPosterContext> options) : base(options)
    { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

        modelBuilder.SeedUsers();
    }
}