using System.Reflection;
using MassTransit;
using Serilog;
using TwitPoster.EmailSender;
using TwitPoster.EmailSender.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

var rabbitMqConfig = builder.Configuration.GetRequiredSection("RabbitMq");
var mailOptions = builder.Configuration.GetRequiredSection("Mail");

builder.Services
    .Configure<MailOptions>(builder.Configuration.GetRequiredSection("Mail"))
    .Configure<RabbitMqTransportOptions>(rabbitMqConfig)
    .AddScoped<IEmailService, EmailService>()
    
    .AddMassTransit(x =>
    {
        var entryAssembly = Assembly.GetExecutingAssembly();
        x.AddConsumers(entryAssembly);
        
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host("Endpoint=sb://twitposter-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=9dXLmG+u5cc3FpBqeKs/WFd1yGWTdID6p+ASbHPSwMg=");
        
            cfg.ConfigureEndpoints(context);
        });
        //mass.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
        
    });

var app = builder.Build();
var mailOption = mailOptions.Get<MailOptions>()!;
app.Logger.LogInformation("mailOptions: {@mailOption}", mailOption);

app.Run();