using Microsoft.Extensions.DependencyInjection;
using TwitPoster.EmailSender.IntegrationTests.Fixtures;

namespace TwitPoster.EmailSender.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public abstract class BaseIntegrationTest(IntegrationTestWebFactory factory) : IAsyncLifetime
{
    protected AsyncServiceScope Scope { get; private set; }
    
    public Task InitializeAsync()
    {
        Scope = factory.Services.CreateAsyncScope();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Scope.DisposeAsync();
        await factory.MailHogContainer.ClearReceivedMessages();
    }
}
    