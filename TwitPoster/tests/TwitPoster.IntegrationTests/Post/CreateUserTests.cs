using FluentAssertions;
using MassTransit.Testing;
using TwitPoster.Shared.Contracts;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.IntegrationTests.Post;

public class CreateUserTests : BaseIntegrationTest
{
    public CreateUserTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Create_Post_should_create_post()
    {
        RegistrationRequest registrationRequest =
            new RegistrationRequest("Oleg", "Besdolla", DateTime.UtcNow.Date.AddYears(-33), "somemail@gmail.com", "String.123-password");
        
        var postsResponse = await ApiClient.PostAsJsonAsync("Auth/registration", registrationRequest);
        var testHarness = Factory.Services.GetTestHarness();
        var sentMessage = testHarness.Published.Select<EmailCommand>().Any();

        postsResponse.Should().Be200Ok();
        sentMessage.Should().BeTrue();
    }

}
