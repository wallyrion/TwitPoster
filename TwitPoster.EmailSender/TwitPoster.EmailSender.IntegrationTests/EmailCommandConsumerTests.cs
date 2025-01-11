using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TwitPoster.EmailSender.IntegrationTests.Fixtures;
using TwitPoster.EmailSender.Options;
using TwitPoster.Shared.Contracts;

namespace TwitPoster.EmailSender.IntegrationTests;

public class EmailCommandConsumerTests(SharedFixtures fixtures) : BaseIntegrationTest( fixtures), IAsyncLifetime
{
    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        await WebApiFactory.RabbitMqContainer.WaitForQueueToBeReady(nameof(EmailCommand));
    }

    [Fact]
    public async Task Should_consume_message_and_send_email()
    {
        // Arrange
        var publishEndpoint = WebApiFactory.RabbitMqPublisher.Provider.GetRequiredService<IPublishEndpoint>();
        var emailBody = $"Hello! Please confirm your email";
        var mailCommand = new EmailCommand("email@gmail.com", "Welcome to TwitPoster! Confirm your email", emailBody, TextFormat.Html);

        var newMessageTask = WebApiFactory.MailHogContainer.WaitForNewMessage();

        // Act
        await publishEndpoint.Publish(mailCommand);
        
        var message = await newMessageTask;

        // Assert
        var from = message.From.Mailbox + "@" + message.From.Domain;
        
        var mailOptions = Scope.ServiceProvider.GetRequiredService<IOptions<MailOptions>>().Value;
        from.Should().Be(mailOptions.SendEmailFrom);

        var toAddress = message.To.Should().ContainSingle().Subject;
        var to = toAddress.Mailbox + "@" + toAddress.Domain;
        to.Should().Be(mailCommand.To);

        message.Content.Headers.Subject.Should().ContainSingle().Which.Should().Be(mailCommand.Subject);

        message.Content.Body.Should().Be(emailBody);
    }
}