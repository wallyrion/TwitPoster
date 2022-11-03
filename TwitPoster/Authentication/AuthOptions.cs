namespace TwitPoster.Authentication;

public static class AuthOptions
{
    public const string Issuer = "TwitPoster"; 
    public const string Audience = "http://localhost:5000/";
    public const string Key = "mysupersecret_secretkey!123";
    public static TimeSpan Expiration = TimeSpan.FromMinutes(30);
}