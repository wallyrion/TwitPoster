using Testcontainers.RabbitMq;

namespace TwitPoster.EmailSender.IntegrationTests.Fixtures;

public class RabbitMqContainerFixture : IAsyncDisposable
{
    public const string Username = "guest";
    public const string Password = "guest";
    
    public RabbitMqContainer Container { get; } = new RabbitMqBuilder()
        .WithUsername(Username)
        .WithPassword(Password)
        .Build();

    public ushort PublicPort => Container.GetMappedPublicPort(5672);
    public string HostName => Container.Hostname;
    
    public async Task StartAsync()
    {
        await Container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}