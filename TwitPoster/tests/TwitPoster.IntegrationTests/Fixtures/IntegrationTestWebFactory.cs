﻿using System.Data.Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
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
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("/opt/mssql-tools18/bin/sqlcmd", "-C", "-Q", "SELECT 1;") )
        .Build();
    public readonly RedisContainer RedisContainer = new RedisBuilder().Build();
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
            Console.WriteLine("Actual azure uri: " + _azure.Uri);
            var collection = new[]
            {
                KeyValuePair.Create("ConnectionStrings:Redis", RedisContainer.GetConnectionString()),
                KeyValuePair.Create("ConnectionStrings:DbConnection", _msSqlContainer.GetConnectionString()),
                KeyValuePair.Create("Secrets:UseSecrets", "false"),
                KeyValuePair.Create("Auth:Secret", "topsecret_secretkey!123_for#TwitPosterApp"),
                KeyValuePair.Create("CountriesApi:Uri", LocationApiServer.Url),
                KeyValuePair.Create("Storage:Uri", _azure.Uri),
                KeyValuePair.Create("Storage:AccountName", AzuriteFixture.AccountName),
                KeyValuePair.Create("Storage:SharedKey", AzuriteFixture.SharedKey),
                KeyValuePair.Create("FeatureManagement:UseRateLimiting", "false"),
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
        await RedisContainer.StartAsync();
        await _azure.InitializeAsync();
        _dbConnection = new SqlConnection(_msSqlContainer.GetConnectionString());
        
        Console.WriteLine("Ms sql connection string = " + _msSqlContainer.GetConnectionString());
        Console.WriteLine("Redis connection string = " + RedisContainer.GetConnectionString());
        Console.WriteLine("Azure connection string = " + _azure.Uri);
        
        HttpClient = CreateClient();
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            TablesToIgnore = [new Table("__EFMigrationsHistory")]
        });
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
        await _azure.DisposeAsync();
        await RedisContainer.DisposeAsync();
        await _dbConnection.DisposeAsync();
    }
}

