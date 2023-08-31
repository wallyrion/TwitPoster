using Microsoft.EntityFrameworkCore;
using TwitPoster.DAL.Models;

namespace TwitPoster.DAL.Seeding;

internal static class InitialUsersSeed
{
    public static ModelBuilder SeedUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasData(
                new User
                {
                    Email = "oleksii.korniienko@twitposter.com",
                    Id = 1,
                    FirstName = "Oleksii",
                    LastName = "Korniienko",
                    BirthDate = new DateTime(1996, 12, 12),
                    CreatedAt = new DateTime(2021, 1, 1)
                },
                new User
                {
                    
                    Email = "admin@twitposter.com",
                    Id = 2,
                    FirstName = "Admin",
                    LastName = "Admin",
                    BirthDate = new DateTime(2000, 1, 1),
                    CreatedAt = new DateTime(2021, 1, 1),
                }, new User
                {
                    Email = "moderator@twitposter.com",
                    Id = 3,
                    FirstName = "Moderator",
                    LastName = "Moderator",
                    BirthDate = new DateTime(2000, 2, 2),
                    CreatedAt = new DateTime(2021, 1, 1),
                }
            );

        modelBuilder.Entity<UserAccount>()
            .HasData(new UserAccount
                {
                    Id = 1,
                    UserId = 1,
                    Password = "Qwerty123",
                    Role = UserRole.DatabaseOwner
                },
                new UserAccount
                {
                    Id = 2,
                    UserId = 2,
                    Password = "Qwerty123",
                    Role = UserRole.Admin
                },
                new UserAccount
                {
                    Id = 3,
                    UserId = 3,
                    Password = "Qwerty123",
                    Role = UserRole.Moderator
                }
            );

        return modelBuilder;
    }
}