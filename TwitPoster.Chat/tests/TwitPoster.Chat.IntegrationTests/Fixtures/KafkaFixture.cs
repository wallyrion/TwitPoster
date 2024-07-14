using Testcontainers.Kafka;

namespace TwitPoster.Chat.IntegrationTests.Fixtures;

public class KafkaFixture : IAsyncDisposable
{
    public KafkaContainer Container { get; } = new KafkaBuilder()
        .Build();


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

