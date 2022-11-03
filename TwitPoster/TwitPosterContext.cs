using Microsoft.EntityFrameworkCore;

namespace TwitPoster;

public sealed class TwitPosterContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();

    public TwitPosterContext (DbContextOptions<TwitPosterContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}