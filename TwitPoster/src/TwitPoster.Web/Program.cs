using MassTransit;
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
            x.UsingRabbitMq();
        else
            x.UsingAzureServiceBus((_, cfg) => cfg.Host(builder.Configuration.GetConnectionString("ServiceBus")!));
    })
    .AddCors(options => options.AddPolicy(WebConstants.Cors.DefaultPolicy, o =>
    {
        o.AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:4200", "https://wallyrion.github.io")
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

app.Logger.LogInformation("Running app in {EnvironmentName} with {ProcessorsCount} processor(s)", app.Environment.EnvironmentName,  Environment.ProcessorCount);

app.Run();