using System.Text.RegularExpressions;
using FluentAssertions;
using MassTransit.Testing;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.Shared.Contracts;
using TwitPoster.Web.ViewModels;

namespace TwitPoster.IntegrationTests.Auth;

public partial class RegisterUserTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory), IAsyncLifetime
{
    private ITestHarness _testHarness = null!;
    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        _testHarness = Scope.ServiceProvider.GetTestHarness();
    }

    [Fact]
    public async Task Register_should_create_user_and_publish_emailCommand()
    {
        var registrationRequest = new RegistrationRequest("Oleg", "Besdolla", DateTime.UtcNow.Date.AddYears(-33), "somemail@gmail.com", "String.123-password");
        
        var postsResponse = await ApiClient.PostAsJsonAsync("Auth/registration", registrationRequest);
        postsResponse.Should().Be200Ok();

        var sentMessage = _testHarness.Published.Select<EmailCommand>().Last().Context.Message;

        sentMessage.Should().NotBeNull();
        sentMessage.To.Should().BeEquivalentTo(registrationRequest.Email);
        sentMessage.Subject.Should().Be("Welcome to TwitPoster! Confirm your email");
        sentMessage.Body.Should().NotBeEmpty();
        sentMessage.Format.Should().Be(TextFormat.Html);
    }
    
    [Fact]
    public async Task Register_should_create_user_with_no_confirmed_email_and_login_should_return_400error()
    {
        var registrationRequest = new RegistrationRequest("Oleg", "Besdolla", DateTime.UtcNow.Date.AddYears(-33), "somemail@gmail.com", "String.123-password");
        
        var registerResponse = await ApiClient.PostAsJsonAsync("Auth/registration", registrationRequest);
        registerResponse.Should().Be200Ok();

        var loginRequest = new LoginRequest(registrationRequest.Email, registrationRequest.Password);
        var loginResponse = await ApiClient.PostAsJsonAsync("Auth/login", loginRequest);
        loginResponse.Should().Be400BadRequest();
    }
    
    [Fact]
    public async Task Login_should_be_successful_after_confirming_email()
    {
        var registrationRequest = new RegistrationRequest("Oleg", "Besdolla", DateTime.UtcNow.Date.AddYears(-33), "somemail@gmail.com", "String.123-password");
        
        var registerResponse = await ApiClient.PostAsJsonAsync("Auth/registration", registrationRequest);
        registerResponse.Should().Be200Ok();
        
        var sentMessage = _testHarness.Published.Select<EmailCommand>().Last().Context.Message;
            
        var regex = GetTokenFromEmailBody();
        var match = regex.Match(sentMessage.Body);

        match.Success.Should().BeTrue();

        var token = match.Groups[1].Value;

        token.Should().NotBeNull();
        Guid.Parse(token).Should().NotBeEmpty();
        
        var confirmEmailResponse = await ApiClient.GetAsync($"Auth/EmailConfirmation?token={token}");
        confirmEmailResponse.Should().Be200Ok();
        
        var loginResponse = await ApiClient.PostAsJsonAsync("Auth/login", new LoginRequest(registrationRequest.Email, registrationRequest.Password));
        loginResponse.Should().Be200Ok()
            .And
            .Satisfy<LoginResponse>(response => response.Should().NotBeNull());

    }

    [GeneratedRegex(@"Token=([\da-fA-F]{8}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{4}-[\da-fA-F]{12})")]
    private static partial Regex GetTokenFromEmailBody();
}
