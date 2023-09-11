using MassTransit;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Options;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.Web;
using TwitPoster.Web.Common;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.Middlewares;
using TwitPoster.Web.WebHostServices;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

IConfigurationSection authConfig = builder.Configuration.GetRequiredSection("Auth");
var authOptions = authConfig.Get<AuthOptions>()!;
builder.Services.Configure<AuthOptions>(authConfig);

var rabbitMqConfig = builder.Configuration.GetRequiredSection("RabbitMq");

builder.Services.AddApplicationInsightsTelemetry();
builder.Services
    .AddSwaggerWithAuthorization()
    .AddEndpointsApiExplorer()
    .AddFluentValidators()
    .AddProblemDetails()
    .AddJwtBearerAuthentication(authOptions)
    .AddMappings()
    .AddDbContext<TwitPosterContext>(options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!))
    .AddScoped<IUsersService, UserService>()
    .AddScoped<IPostService, PostService>()
    .AddScoped<ICurrentUser, CurrentUser>()
    .AddScoped<IAuthService, AuthService>()
    .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()

    .Configure<RabbitMqTransportOptions>(rabbitMqConfig)
    
    .AddMassTransit(x =>
    {
        if (builder.Environment.IsDevelopment())
        {
            Log.Logger.Information("Adding mass transit with rabbitmq");
            x.UsingRabbitMq();
        }
        else
        {
            Log.Logger.Information("Adding mass transit with azure service bus");

            x.UsingAzureServiceBus((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("ServiceBus")!;

                Log.Logger.Information("ServiceBus connection string: {connectionString}", connectionString);
                cfg.Host(connectionString);
            });
        }
    })
    .AddCors(options => options.AddPolicy(WebConstants.Cors.DefaultPolicy, o =>
    {
        o.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4200")
            .AllowCredentials();
    }))
    .AddHostedService<MigrationHostedService>()
    .AddHostedService<TestBackgroundService>()
    
    .AddStackExchangeRedisCache(x =>
    {
        x.Configuration = builder.Configuration.GetConnectionString("Redis")!;
    })
    ;



var app = builder.Build();

app.MapControllers()
    .RequireAuthorization();

app
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

app.Logger.LogInformation("Starting application with {ProcessorsCount} processor(s)", Environment.ProcessorCount);

app.Run();