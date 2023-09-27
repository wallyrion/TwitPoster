using System.Data.Common;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Respawn;
using Respawn.Graph;
using Testcontainers.Azurite;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using TwitPoster.IntegrationTests.ExternalApis;
using TwitPoster.Web;

namespace TwitPoster.IntegrationTests;

public class AzuriteFixture : IAsyncLifetime
{
    public const string AccountName = "devstoreaccount1";
    public const string SharedKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
    public const string BlobPort = "10000";
    public string Host => _azuriteContainer.Hostname;
    public string Uri => $"http://{Host}:{BlobPort}/{AccountName}";
    
    
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        .Build();

    public async Task InitializeAsync()
    {
        await _azuriteContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _azuriteContainer.DisposeAsync();
    }
}

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