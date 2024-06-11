using Serilog;
using TwitPoster.EmailSender.Extensions;
using TwitPoster.EmailSender.Options;
using TwitPoster.EmailSender.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

builder.Configuration.BindOption<MailOptions>(builder.Services);

builder.Services
    .AddScoped<IEmailService, EmailService>()
    .AddMessaging(builder.Configuration);

var app = builder.Build();

app.MapGet("/health", () => "OK");

app.Run();