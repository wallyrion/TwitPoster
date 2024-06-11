using Microsoft.Extensions.DependencyInjection;
using TwitPoster.EmailSender.IntegrationTests.Fixtures;

namespace TwitPoster.EmailSender.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public abstract class BaseIntegrationTest(IntegrationTestWebFactory factory) : IAsyncLifetime
{
    public AsyncServiceScope Scope { get; private set; }
    
    public async Task InitializeAsync()
    {
        var client = factory.CreateClient();
        Scope = factory.Services.CreateAsyncScope();
    }

    public async Task DisposeAsync()
    {
        await Scope.DisposeAsync();
        await factory.MailHogContainer.ClearReceivedMessages();
    }
}
    