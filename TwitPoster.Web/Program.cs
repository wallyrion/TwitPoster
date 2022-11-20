using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TwitPoster.BLL.Interfaces;
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

builder.Services

    .AddSwaggerWithAuthorization()
    .AddEndpointsApiExplorer()

    .AddFluentValidators()
    .AddProblemDetails()
    .AddJwtBearerAuthentication()
    .AddMappings()
    
    .AddDbContext<TwitPosterContext>(options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!))
    .AddScoped<IUsersService, UserService>()
    .AddScoped<IPostService, PostService>()
    .AddScoped<ICurrentUser, CurrentUser>()
    .AddOutputCache()
    ;

var app = builder.Build();

app.Logger.LogInformation("Starting application with {ProcessorsCount} processor(s)", Environment.ProcessorCount);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<TwitPosterContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        app.Logger.LogInformation("Migrating database....");
        context.Database.Migrate();
        app.Logger.LogInformation("Database migrated");
    }
}

app.MapControllers()
    .RequireAuthorization();

app
    .UseSwagger().UseSwaggerUI()
    /*mssqlDb.InDevelopment(b =>
        b.UseSwagger().UseSwaggerUI())
        */
    
    .UseOutputCache()
    .UseMiddleware<RequestDurationMiddleware>()
    .Use(CustomMiddlewares.ExtendRequestDurationMiddleware)
    
    .UseSerilogRequestLogging()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseExceptionHandler()
    .UseStatusCodePages();

app.InDevelopment(b =>
        b.UseDeveloperExceptionPage())
    .UseMiddleware<BusinessValidationMiddleware>()
    .UseMiddleware<SetupUserClaimsMiddleware>();

app.Run();