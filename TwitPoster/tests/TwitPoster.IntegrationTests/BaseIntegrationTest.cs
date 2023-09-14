using System.Net.Http.Headers;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.DAL.Models;
using TwitPoster.IntegrationTests.TestData;

namespace TwitPoster.IntegrationTests;

[Collection(nameof(SharedTestCollection))]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly IntegrationTestWebFactory Factory;
    protected readonly IServiceScope Scope;
    protected readonly TwitPosterContext DbContext;
    protected readonly HttpClient ApiClient;
    
    protected int DefaultUserId;

    protected readonly IntegrationData Data;

    public BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        ApiClient = factory.HttpClient;
        DbContext = Scope.ServiceProvider.GetRequiredService<TwitPosterContext>();
        
        Data = new IntegrationData(DbContext);
    }

    protected async Task AddAuthorization()
    {
        var user = await DbContext.Users.Include(x => x.UserAccount).FirstAsync(user => user.Id == DefaultUserId);

        await AddAuthorization(ApiClient, user.Id);
    }

    protected async Task<IReadOnlyList<(HttpClient apiClient, User user)>> CreateConcurrentClients(int usersCount = 10)
    {
        var users = await Data.AddMany<User>(usersCount);

        var clients = new List<(HttpClient, User)>();
        
        foreach (var user in users)
        {
            var client = Factory.CreateClient();
            await AddAuthorization(client, user.Id);
            clients.Add((client,user ));
        }

        return clients;
    }
    protected async Task AddAuthorization(HttpClient httpClient, int userId)
    {
        var user = await DbContext.Users.Include(x => x.UserAccount).FirstAsync(user => user.Id == userId);
        var jwtGenerator = Scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
        var token = jwtGenerator.GenerateToken(user);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task InitializeAsync()
    {
        await AddDefaultUser();
    }

    private async Task AddDefaultUser()
    {
        var user = Data.BaseFixture.Create<User>();
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        DefaultUserId = user.Id;
        
        Data.Initialize(user.Id);
    }

    public async Task DisposeAsync()
    {
        await Factory.ResetDatabaseAsync();
        ApiClient.DefaultRequestHeaders.Authorization = null;
    }
}
    