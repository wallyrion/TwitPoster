using System.Net.Http.Headers;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitPoster.BLL.Interfaces;
using TwitPoster.DAL;
using TwitPoster.IntegrationTests.Fixtures;
using TwitPoster.IntegrationTests.TestData;
namespace TwitPoster.IntegrationTests;

[Collection(nameof(SharedTestCollection))]
public abstract class BaseIntegrationTest(IntegrationTestWebFactory factory) : IAsyncLifetime
{
    protected readonly IntegrationTestWebFactory Factory = factory;
    protected readonly HttpClient ApiClient = factory.HttpClient;
    protected AsyncServiceScope Scope { get; private set; }
    protected TwitPosterContext DbContext { get; private set; } = null!;
    
    protected int DefaultUserId;

    protected readonly IntegrationData Data = new();

    protected async Task AddAuthorization()
    {
        var user = await DbContext.Users.Include(x => x.UserAccount).FirstAsync(user => user.Id == DefaultUserId);

        await AddAuthorization(ApiClient, user.Id);
    }

    protected async Task<IReadOnlyList<(HttpClient apiClient, DAL.Models.User user)>> CreateConcurrentClients(int usersCount = 10)
    {
        var users = await AddMany<DAL.Models.User>(usersCount);

        var clients = new List<(HttpClient, DAL.Models.User)>();
        
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
        
        Scope = Factory.Services.CreateAsyncScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<TwitPosterContext>();
    }

    private async Task AddDefaultUser()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TwitPosterContext>();
        var user = Data.BaseFixture.Create<DAL.Models.User>();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        DefaultUserId = user.Id;
        
        Data.Initialize(user.Id);
    }

    public async Task DisposeAsync()
    {
        await Scope.DisposeAsync();
        await Factory.ResetDatabaseAsync();
        ApiClient.DefaultRequestHeaders.Authorization = null;
    }
    
    public async Task<IReadOnlyList<T>> AddMany<T>(int count = 3) where T : class
    {
        var entities = Data.BaseFixture.CreateMany<T>(count).ToList();
        DbContext.Set<T>().AddRange(entities);
        await DbContext.SaveChangesAsync();

        return entities;
    }
    
    public async Task<IReadOnlyList<T>> AddMany<T>(IReadOnlyList<T> entitiesToAdd) where T : class
    {
        DbContext.Set<T>().AddRange(entitiesToAdd);
        await DbContext.SaveChangesAsync();
        return entitiesToAdd;
    }
}
    