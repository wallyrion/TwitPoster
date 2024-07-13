using TwitPoster.Chat;
using TwitPoster.Chat.Infrastructure;
using TwitPoster.Chat.Infrastructure.Auth;
using TwitPoster.Chat.Infrastructure.SignalR;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwagger();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SetupUserClaimsMiddleware>();

app.MapGet("/weatherforecast", () =>
    {
       
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapControllers();
app.MapHub<NotificationHub>(NotificationHub.EndpointPath);
app.Run();