using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TwitPoster.BLL.Authentication;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Options;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.Web;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

builder.Services.AddControllers();

IConfigurationSection authConfig = builder.Configuration.GetRequiredSection("Auth");
var authOptions = authConfig.Get<AuthOptions>()!;
builder.Services.Configure<AuthOptions>(authConfig);

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
    .AddMassTransit(mass => mass.UsingRabbitMq((_, cfg) =>
        cfg.Host("my-rabbit", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        })));

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", o =>
{
    o.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowCredentials();
}));

var app = builder.Build();

app.Logger.LogInformation("Starting application with {ProcessorsCount} processor(s)", Environment.ProcessorCount);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TwitPosterContext>();
    var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToList();
    if (pendingMigrations.Any())
    {
        app.Logger.LogInformation("Migrating database.... {PendingMigrations} pending migrations", JsonSerializer.Serialize(pendingMigrations));
        context.Database.Migrate();
        app.Logger.LogInformation("Database migrated");
    }
}

app.Logger.LogInformation("After migration application with {ProcessorsCount} processor(s)", Environment.ProcessorCount);


app.MapControllers()
    .RequireAuthorization();

app
    .UseSwagger().UseSwaggerUI()
    /*mssqlDb.InDevelopment(b =>
        b.UseSwagger().UseSwaggerUI())
        */

    .UseCors("CorsPolicy")
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

app.Logger.LogInformation("Starting app...");


app.Run();