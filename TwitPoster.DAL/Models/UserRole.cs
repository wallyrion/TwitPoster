namespace TwitPoster.DAL.Models;

public enum UserRole
{
    User = 10,
    Moderator = 20,
    Admin = 50,
    DatabaseOwner = 100,
}

public static class UserRoles
{
    public const string User = "User";
    public const string Moderator = "Moderator";
    public const string Admin = "Admin";
    public const string DatabaseOwner = "DatabaseOwner";
}