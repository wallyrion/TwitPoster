using System.Reflection;
using MassTransit;
using Serilog;
using TwitPoster.EmailSender;
using TwitPoster.EmailSender.Consumer;
using TwitPoster.EmailSender.Services;
using TwitPoster.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

var rabbitMqConfig = builder.Configuration.GetRequiredSection("RabbitMq");
var mailOptions = builder.Configuration.GetRequiredSection("Mail");

builder.Services
    .Configure<MailOptions>(mailOptions)
    //.Configure<RabbitMqTransportOptions>(rabbitMqConfig)
    .AddScoped<IEmailService, EmailService>()
    
    .AddMassTransit(x =>
    {
        var entryAssembly = Assembly.GetExecutingAssembly();
        x.AddConsumers(entryAssembly);

        /*if (builder.Environment.IsDevelopment())
        {
            x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
        }{*/
        
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetConnectionString("ServiceBus"));
            
            cfg.UseServiceBusMessageScheduler();
            cfg.SubscriptionEndpoint<EmailCommand>($"EmailSenderSubscription" , configurator =>
            {
                configurator.ConfigureConsumer<EmailCommandConsumer>(context);
            });
        });      
    });

var app = builder.Build();

app.MapGet("/health", () => "OK");
app.MapGet("/health2", () => "OK");
app.Run();