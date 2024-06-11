using System.Diagnostics;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.RabbitMq;
using TwitPoster.EmailSender.IntegrationTests.Fixtures;
using TwitPoster.EmailSender.Options;
using TwitPoster.Shared.Contracts;
using Xunit.Abstractions;

namespace TwitPoster.EmailSender.IntegrationTests;

public class UniTest
{
    [Fact]
    public async Task Test1()
    {
        var rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management")
            .WithPortBinding(15672, true)
            .WithUsername("guest").WithPassword("guest").Build();
        
        await rabbitMqContainer.StartAsync();
        var fakeRabbitMqPublisher = new FakeRabbitMqPublisher(rabbitMqContainer);

        await Task.Delay(1000);
        
        var publishEndpoint = fakeRabbitMqPublisher.Provider.GetRequiredService<IPublishEndpoint>();
        var mailCommand = new EmailCommand("email@gmail.com", "Welcome to TwitPoster! Confirm your email", "", TextFormat.Html);
        await Task.Delay(1000);

        await publishEndpoint.Publish(mailCommand);
        Console.WriteLine("Publishing email");

        await Task.Delay(1000);

        await Task.Delay(10000);
    }
}

public class UnitTest1(IntegrationTestWebFactory factory, ITestOutputHelper output) : BaseIntegrationTest(factory)
{
    private readonly IntegrationTestWebFactory _factory = factory;


    [Fact]
    public async Task Test1()
    {
        //output.WriteLine("Test1 from xunit output");
        Debug.WriteLine("Test1");
        Console.WriteLine("Test1");
        var mailOptions = Scope.ServiceProvider.GetRequiredService<IOptions<MailOptions>>().Value;

        Console.WriteLine("GetRequiredService<IPublishEndpoint>()");
        await Task.Delay(100);
        var publishEndpoint = _factory.rabbitMqPublisher.Provider.GetRequiredService<IPublishEndpoint>();

        var emailBody = $"""
                             <h1> You are on the way! </h1>
                             <h2> Please confirm your email by clicking on the link below </h2>
                             <a href="https://localhost:7267/Auth/EmailConfirmation?Token={Guid.NewGuid()}">Press to confirm email address</a>
                         """;

        var mailCommand = new EmailCommand("email@gmail.com", "Welcome to TwitPoster! Confirm your email", emailBody, TextFormat.Html);

        Console.WriteLine("Publishing email");
        await publishEndpoint.Publish(mailCommand);
        Console.WriteLine("Email published");
        await Task.Delay(100);

        //await publishEndpoint.Publish(mailCommand);

        var response = await factory.MailHogContainer.GetReceivedMessages();

        var message = response.Items.Should().ContainSingle().Subject;

        var from = message.From.Mailbox + "@" + message.From.Domain;
        from.Should().Be(mailOptions.SendEmailFrom);

        var toAddress = message.To.Should().ContainSingle().Subject;
        var to = toAddress.Mailbox + "@" + toAddress.Domain;
        to.Should().Be(mailCommand.To);

        message.Content.Headers.Subject.Should().ContainSingle().Which.Should().Be(mailCommand.Subject);
    }
}