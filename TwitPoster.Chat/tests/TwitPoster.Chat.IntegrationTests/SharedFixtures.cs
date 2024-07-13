using Testcontainers.MongoDb;

namespace TwitPoster.Chat.IntegrationTests;

[Collection(nameof(SharedTestCollection))]
public class SharedFixtures : IAsyncLifetime
{
    public MongoDbContainer MongoContainer { get; } = new MongoDbBuilder().Build();
    
    
    public async Task InitializeAsync()
    {
        await MongoContainer.StartAsync();
    }
    
    public async Task DisposeAsync()
    {
        await MongoContainer.DisposeAsync();
    }
}