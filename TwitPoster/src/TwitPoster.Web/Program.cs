using Azure.Identity;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Serilog;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Common.Options;
using TwitPoster.BLL.Extensions;
using TwitPoster.BLL.External.Location;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.DAL.Triggers;
using TwitPoster.Web;
using TwitPoster.Web.Common;
using TwitPoster.Web.Common.DependencyInjection;
using TwitPoster.Web.Common.Options;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.Middlewares;
using TwitPoster.Web.WebHostServices;

try
{
    
    var builder = WebApplication.CreateBuilder(args);
    
    var secrets = builder.Configuration.BindOption<SecretOptions>(builder.Services, false);

    if (secrets.UseSecrets)
    {
        builder.Configuration.AddAzureKeyVault(new Uri(secrets.KeyVaultUri), new ClientSecretCredential(secrets.TenantId, secrets.ClientId, secrets.ClientSecret));
    }
    
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    builder.Host.UseSerilog(Log.Logger);

    builder.Services.AddControllers();

    var authConfig = builder.Configuration.BindOption<AuthOptions>(builder.Services);
    var connectionStrings = builder.Configuration.BindOption<ConnectionStringsOptions>(builder.Services);

    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddFeatureManagement();
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
        .AddTwitPosterCaching(builder.Configuration)
        .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()

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
                .WithOrigins("http://localhost:4200", "https://wallyrion.github.io")
                .AllowCredentials();
        }))

        .AddHostedService<MigrationHostedService>();
    //.AddHostedService<TestBackgroundService>()

    builder.Services.AddHttpClient<ILocationClient, LocationClient>(client
        => client.BaseAddress = new Uri("https://countriesnow.space/"));

    var app = builder.Build();

    app.MapGet("/health", () => "OK");

    app.MapGet(".well-known/acme-challenge/{file}", () => "something");
    app.MapControllers()
        .RequireAuthorization();

    
    app
        .UseStaticFiles()
        .UseSwagger().UseSwaggerUI()

        .UseCors(WebConstants.Cors.DefaultPolicy)
        .UseMiddleware<RequestDurationMiddleware>()
        .Use(CustomMiddlewares.ExtendRequestDurationMiddleware)
        .UseSerilogRequestLogging()
        .UseAuthentication()
        .UseAuthorization()

        .UseExceptionHandler()
        .UseStatusCodePages();

    app.InDevelopment(b =>
            b.UseDeveloperExceptionPage())
        .UseMiddleware<BusinessValidationMiddleware>()
        .UseMiddleware<SetupUserClaimsMiddleware>();

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