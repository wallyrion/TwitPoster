using System.Reflection;
using MassTransit;
using Serilog;
using TwitPoster.EmailSender;
using TwitPoster.EmailSender.Services;

var builder = WebApplication.CreateBuilder(args);
var rabbitMqConfig = builder.Configuration.GetRequiredSection("RabbitMq");

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());
builder.Services
    .Configure<MailOptions>(builder.Configuration.GetRequiredSection("Mail"))
    .Configure<RabbitMqTransportOptions>(rabbitMqConfig)
    .AddScoped<IEmailService, EmailService>()
    
    .AddMassTransit(mass =>
    {
        var entryAssembly = Assembly.GetExecutingAssembly();
        mass.AddConsumers(entryAssembly);
        
        mass.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
    });

var app = builder.Build();

app.Run();