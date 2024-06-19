using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using TwitPoster.EmailSender.Extensions;
using TwitPoster.EmailSender.Options;
using TwitPoster.EmailSender.Services;

try
{
    var builder = WebApplication.CreateBuilder(args);

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

    builder.Services.AddApplicationInsightsTelemetry();
    builder.Configuration.BindOption<MailOptions>(builder.Services);

    builder.Services
        .AddScoped<IEmailService, EmailService>()
        .AddMessaging(builder.Configuration);

    var app = builder.Build();

    app.MapGet("/health", () => "OK");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while processing the request");
}
finally
{
    Log.Information("Shutting down the application...");
}

