using FluentAssertions;
using Xunit.Abstractions;

namespace TwitPoster.AutoTests;

public static class Configuration
{
    public static string TwitposterApiBaseUrl => Environment.GetEnvironmentVariable("TWITPOSTER_API_BASE_URL") ?? "https://twitposter-production-appservice.azurewebsites.net";
    public static string TestUserEmail => Environment.GetEnvironmentVariable("TEST_USER_EMAIL") ?? "oleksii.korniienko@twitposter.com";
    public static string TestUserPassword => Environment.GetEnvironmentVariable("TEST_USER_EMAIL") ?? "Qwerty123";
}

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test1()
    {
        _testOutputHelper.WriteLine("TWITPOSTER_API_BASE_URL " + Environment.GetEnvironmentVariable("TWITPOSTER_API_BASE_URL"));
        Console.WriteLine("TWITPOSTER_API_BASE_URL " + Environment.GetEnvironmentVariable("TWITPOSTER_API_BASE_URL"));
        
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Configuration.TwitposterApiBaseUrl);

        var response = await httpClient.GetAsync("posts");

        response.Should().Be200Ok();
    }
}