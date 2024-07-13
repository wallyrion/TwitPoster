using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TwitPoster.Chat.IntegrationTests;

public class IntegrationTestWebFactory : WebApplicationFactory<IApiMarker>
{
    public readonly string Secret = "supersecretkey" + Guid.NewGuid();
    private readonly SharedFixtures _fixtures;
    
    public IntegrationTestWebFactory(SharedFixtures fixtures)
    {
        _fixtures = fixtures;
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(x =>
        {
            var collection = new[]
            {
                KeyValuePair.Create("TwitPosterChatDb:ConnectionString", _fixtures.MongoContainer.GetConnectionString()),
                KeyValuePair.Create("Auth:Secret", Secret)
            };
            x.AddInMemoryCollection(collection!);
        });
        
        return base.CreateHost(builder);
    }
}