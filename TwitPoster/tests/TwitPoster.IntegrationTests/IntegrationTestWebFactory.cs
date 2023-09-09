using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.MsSql;
using TwitPoster.DAL;
using TwitPoster.Web;

namespace TwitPoster.IntegrationTests;

public class IntegrationTestWebFactory : WebApplicationFactory<IApiTestMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public HttpClient HttpClient { get; private set; } = null!;
    private DbConnection _dbConnection = null!;
    private Respawner _respawner = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TwitPosterContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TwitPosterContext>(options => options.UseSqlServer(_msSqlContainer.GetConnectionString()));
        });
    }
    
    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        _dbConnection = new SqlConnection(_msSqlContainer.GetConnectionString());
        
        HttpClient = CreateClient();
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new []{ "dbo" },
        });
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}