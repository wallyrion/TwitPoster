using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TwitPoster.BLL.Interfaces;
using TwitPoster.BLL.Services;
using TwitPoster.DAL;
using TwitPoster.Web.Extensions;
using TwitPoster.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    ;

builder.Services

    .AddSwaggerWithAuthorization()
    .AddEndpointsApiExplorer()

    .AddFluentValidators()
    .AddProblemDetails()
    .AddJwtBearerAuthentication()

    .AddDbContext<TwitPosterContext>(options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!))
    .AddScoped<IUsersService, UserService>()
    .AddScoped<IPostService, PostService>()
    .AddScoped<ICurrentUser, CurrentUser>()
    ;

var app = builder.Build();

app.MapControllers()
    .RequireAuthorization();

app
    .InDevelopment(b =>
        b.UseSwagger().UseSwaggerUI())
    
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

