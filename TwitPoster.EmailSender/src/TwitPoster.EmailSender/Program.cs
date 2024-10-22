using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;
using TwitPoster.EmailSender.Extensions;
using TwitPoster.EmailSender.Options;
using TwitPoster.EmailSender.Services;
using TwitPoster.Shared.Contracts;

try
{
    var builder = WebApplication.CreateBuilder(args);

    /*
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateBootstrapLogger();
        */
    
    builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
    {
        options.EnableLiveMetrics = false;
    });
    builder.Services.ConfigureOpenTelemetryTracerProvider((sp, b) =>
    {
        b.AddSource("MassTransit");
    });
    


    //builder.Services.AddApplicationInsightsTelemetry();
    builder.Configuration.BindOption<MailOptions>(builder.Services);

    builder.Services
        .AddScoped<IEmailService, EmailService>()
        .AddMessaging(builder.Configuration);

    var app = builder.Build();

    app.MapGet("test-email", async (IEmailService emailService) =>
    {
        await emailService.SendEmail(new EmailCommand("kornienko1296@gmail.com", "test email", "Hy this is test email", TextFormat.Html));
    });
    
    app.MapGet("/health", () => "OK");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine( "An error occurred while processing the request" + ex);
}
finally
{
    Console.WriteLine("Shutting down the application...");
}

