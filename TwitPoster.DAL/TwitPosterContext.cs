using Microsoft.EntityFrameworkCore;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL;

public sealed class TwitPosterContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();

    public TwitPosterContext (DbContextOptions<TwitPosterContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}