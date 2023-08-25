using System.Reflection;
using MassTransit;
using TwitPoster.EmailSender;
using TwitPoster.EmailSender.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure<MailOptions>(builder.Configuration.GetRequiredSection("Mail"))
    .AddScoped<IEmailService, EmailService>()
    .AddMassTransit(mass =>
    {
        var entryAssembly = Assembly.GetExecutingAssembly();
        mass.AddConsumers(entryAssembly);

        mass.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
    });

var app = builder.Build();

app.Run();