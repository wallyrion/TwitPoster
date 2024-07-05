namespace TwitPoster.AutoTests;

public static class Configuration
{
    public static string TwitposterApiBaseUrl => Environment.GetEnvironmentVariable("TWITPOSTER_API_BASE_URL") ?? "https://twitposter-production-appservice.azurewebsites.net";
    public static string TestUserEmail => Environment.GetEnvironmentVariable("TEST_USER_EMAIL") ?? "oleksii.korniienko@twitposter.com";
    public static string TestUserPassword => Environment.GetEnvironmentVariable("TEST_USER_EMAIL") ?? "Qwerty123";
}