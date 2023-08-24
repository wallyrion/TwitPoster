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
builder.Services.Configure<MailOptions>(builder.Configuration.GetRequiredSection("Mail"));

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
    .AddScoped<IEmailSender, EmailSender>()
    .AddScoped<IAuthService, AuthService>()
    .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()
    .AddOutputCache();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", o =>
{
    o.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowCredentials();
}));

var app = builder.Build();

app.MapControllers()
    .RequireAuthorization();

app
    .InDevelopment(b =>
        b.UseSwagger().UseSwaggerUI())
    
    .UseCors("CorsPolicy")
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