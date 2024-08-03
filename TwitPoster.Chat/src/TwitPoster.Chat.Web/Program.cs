using MassTransit;
using Microsoft.AspNetCore.Mvc;
using TwitPoster.Chat;
using TwitPoster.Chat.Application;
using TwitPoster.Chat.Application.Messages.Events;
using TwitPoster.Chat.Infrastructure;
using TwitPoster.Chat.Infrastructure.Auth;
using TwitPoster.Chat.Infrastructure.SignalR;

Console.WriteLine("Initializing the application...");

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Configuring the application...");


builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSwagger();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddPolicy(WebConstants.Cors.DefaultPolicy, o =>
{
    o.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("http://localhost:4200", "https://wallyrion.github.io", "https://twitposter.xyz")
        .AllowCredentials();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(WebConstants.Cors.DefaultPolicy);
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();


app.UseAuthorization();
app.UseMiddleware<SetupUserClaimsMiddleware>();

app.MapGet("/weatherforecast", async ([FromQuery] string chatId, [FromServices] ITopicProducer<string, MessageAddedToChatEvent> topicProducer) =>
    {
        return Results.Ok();
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();



app.MapControllers();
app.MapHub<ConversationHub>(ConversationHub.EndpointPath);
app.Run();