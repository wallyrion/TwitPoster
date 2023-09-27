using FluentAssertions;
using MassTransit.Testing;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Shared.Contracts;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.IntegrationTests.Auth;

public class RegisterUserTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Create_user_should_create_user_and_publish_emailCommand()
    {
        var registrationRequest = new RegistrationRequest("Oleg", "Besdolla", DateTime.UtcNow.Date.AddYears(-33), "somemail@gmail.com", "String.123-password");
        
        var postsResponse = await ApiClient.PostAsJsonAsync("Auth/registration", registrationRequest);
        postsResponse.Should().Be200Ok();
        var testHarness = Factory.Services.GetTestHarness();
        var sentMessage = testHarness.Published.Select<EmailCommand>().Should().ContainSingle().Subject.Context.Message;

        sentMessage.To.Should().BeEquivalentTo(registrationRequest.Email);
        sentMessage.Subject.Should().Be("Welcome to TwitPoster! Confirm your email");
        sentMessage.Body.Should().NotBeEmpty();
        sentMessage.Format.Should().Be(TextFormat.Html);
    }

}
