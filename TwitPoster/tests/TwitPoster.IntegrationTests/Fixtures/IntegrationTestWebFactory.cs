using System.Data.Common;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using TwitPoster.IntegrationTests.ExternalApis;
using TwitPoster.Web;

namespace TwitPoster.IntegrationTests.Fixtures;

public class IntegrationTestWebFactory : WebApplicationFactory<IApiTestMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    private readonly AzuriteFixture _azure = new();
    
    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;
    
    public HttpClient HttpClient { get; private set; } = null!;
    public readonly LocationApiServer LocationApiServer = new(); 
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(x =>
        {
            var collection = new[]
            {
                KeyValuePair.Create("ConnectionStrings:Redis", _redisContainer.GetConnectionString()),
                KeyValuePair.Create("ConnectionStrings:DbConnection", _msSqlContainer.GetConnectionString()),
                KeyValuePair.Create("Secrets:UseSecrets", "false"),
                KeyValuePair.Create("Auth:Secret", "topsecret_secretkey!123_for#TwitPosterApp"),
                KeyValuePair.Create("CountriesApi:Uri", LocationApiServer.Url),
                KeyValuePair.Create("Storage:Uri", _azure.Uri),
                KeyValuePair.Create("Storage:AccountName", AzuriteFixture.AccountName),
                KeyValuePair.Create("Storage:SharedKey", AzuriteFixture.SharedKey),
            };
            x.AddInMemoryCollection(collection!);
        });
        
        return base.CreateHost(builder);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _azure.InitializeAsync();
        _dbConnection = new SqlConnection(_msSqlContainer.GetConnectionString());
        LocationApiServer.Start();
        
        HttpClient = CreateClient();
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new []{ "dbo" },
            TablesToIgnore = new []{new Table("__EFMigrationsHistory")}
        });
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}