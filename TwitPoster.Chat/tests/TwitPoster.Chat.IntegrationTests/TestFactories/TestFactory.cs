namespace TwitPoster.Chat.IntegrationTests.TestFactories;

public static class TestFactory
{
    public static TestUser CreateUser(int id = 1, string firstName = "Oleksii", string lastName = "Kornienko", string email = "oleksii.korniienko@twitposter.com")
    {
        return new TestUser
        {
            Id = 1,
            FirstName = firstName,
            LastName = lastName,
            Email = email
        };
    }
}