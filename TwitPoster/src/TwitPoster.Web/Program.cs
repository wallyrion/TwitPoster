using Azure.Identity;
using Azure.Storage;
using MassTransit;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.FeatureManagement;
using Refit;
using Serilog;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Common.Options;
using TwitPoster.BLL.Extensions;
using TwitPoster.BLL.External.Location;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Notifications;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Triggers;
using TwitPoster.Web;
using TwitPoster.Web.Common;
using TwitPoster.Web.Common.DependencyInjection;
using TwitPoster.Web.Common.Options;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.Middlewares;
using TwitPoster.Web.Notifications;
using TwitPoster.Web.WebHostServices;

try
{
    Console.WriteLine("Initializing app builder...");
    var builder = WebApplication.CreateBuilder(args);
    Console.WriteLine("Created app builder...");

    builder.Configuration.AddEnvironmentVariables();
    
    Console.WriteLine("Current image tag: " + builder.Configuration["CurrentImageTag"]);
    
    var secrets = builder.Configuration.BindOption<SecretOptions>(builder.Services, false);

    if (secrets.UseSecrets)
    {
        builder.Configuration.AddAzureKeyVault(new Uri(secrets.KeyVaultUri), new ClientSecretCredential(secrets.TenantId, secrets.ClientId, secrets.ClientSecret));
    }
    
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateBootstrapLogger();
    
    builder.Host.UseSerilog((context, provider, logger) =>
    {
        logger.ReadFrom.Configuration(context.Configuration);

        logger.WriteTo.ApplicationInsights(
            provider.GetRequiredService<TelemetryConfiguration>(),
            TelemetryConverter.Traces);
    });
    
    builder.Services.AddAzureClients(clientBuilder =>
    {
        var options = builder.Configuration.BindOption<StorageOptions>(builder.Services);
        clientBuilder.AddBlobServiceClient(new Uri(options.Uri), new StorageSharedKeyCredential(options.AccountName, options.SharedKey));
    });
    
    builder.Services.AddControllers();

    builder.Configuration.BindOption<ApplicationOptions>(builder.Services);
    
    var authConfig = builder.Configuration.BindOption<AuthOptions>(builder.Services);
    var connectionStrings = builder.Configuration.BindOption<ConnectionStringsOptions>(builder.Services);
    var countriesApiOptions = builder.Configuration.BindOption<CountriesApiOptions>(builder.Services);

    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddFeatureManagement();
    builder.Services
        .AddRefitClient<ILocationClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(countriesApiOptions.Uri))
        .AddStandardResilienceHandler();
        
    builder.Services
        .AddSwaggerWithAuthorization()
        .AddEndpointsApiExplorer()
        .AddFluentValidators()
        .AddProblemDetails()
        .AddJwtBearerAuthentication(authConfig)
        .AddMappings()
        .AddDbContext<TwitPosterContext>(options => options
            .UseTriggers(o => o.AddTrigger<PostLikeTrigger>())
            .UseSqlServer(connectionStrings.DbConnection))
        .AddScoped<IUsersService, UserService>()
        .AddScoped<IPostService, PostService>()
        .AddScoped<ICurrentUser, CurrentUser>()
        .AddScoped<IAuthService, AuthService>()
        .AddScoped<ILocationService, LocationService>()
        .AddScoped<INotificationReporter, HubNotificationsReporter>()
        .AddTwitPosterCaching(builder.Configuration)
        .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()
        .AddTwitPosterRateLimiting(builder.Configuration)

        .Configure<RabbitMqTransportOptions>(builder.Configuration.GetRequiredSection("RabbitMq"))

        .AddMassTransit(x =>
        {
            var featureFlags = builder.Configuration.BindOption<FeatureFlagsOptions>(builder.Services);

            if (featureFlags.UseRabbitMq)
            {
                x.UsingRabbitMq();
            }
            else
            {
                x.UsingAzureServiceBus((_, cfg) =>
                {
                    cfg.UseServiceBusMessageScheduler();

                    cfg.Host(connectionStrings.ServiceBus);
                });
            }
        })
        .AddCors(options => options.AddPolicy(WebConstants.Cors.DefaultPolicy, o =>
        {
            o.AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins("http://localhost:4200", "https://wallyrion.github.io", "https://twitposter.xyz")
                .AllowCredentials();
        }))

        .AddHostedService<MigrationHostedService>()
        .AddSignalR();

    var app = builder.Build();

    app.MapGet("/health", (IConfiguration configuration) => new { ImageTag =  configuration["CurrentImageTag"] });
    app.MapGet(".well-known/acme-challenge/{file}", () => "something");
    app.MapControllers()
        .RequireAuthorization();

    var features = builder.Configuration.BindOption<FeatureFlagsOptions>(builder.Services, false);

    if (features.UseRateLimiting)
    {
        app.UseRateLimiter();    
    }

    app
        .UseSwagger().UseSwaggerUI()

        .UseCors(WebConstants.Cors.DefaultPolicy)
        .UseMiddleware<RequestDurationMiddleware>()
        .UseSerilogRequestLogging()
        .UseAuthentication()
        .UseAuthorization()

        .UseExceptionHandler()
        .UseStatusCodePages();

    app.InDevelopment(b =>
            b.UseDeveloperExceptionPage())
        .UseMiddleware<BusinessValidationMiddleware>()
        .UseMiddleware<SetupUserClaimsMiddleware>();
    
    app.MapHub<NotificationHub>(NotificationHub.EndpointPath);
    
    app.Logger.LogInformation("Running app in {EnvironmentName} with {ProcessorsCount} processor(s)", app.Environment.EnvironmentName,  Environment.ProcessorCount);
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Error while starting app..." + ex.Message);
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Host shutting down...");
    Log.CloseAndFlush();
}
