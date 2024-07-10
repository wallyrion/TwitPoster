using TwitPoster.BLL.Extensions;
using TwitPoster.Chat;
using TwitPoster.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));

builder.Services.AddSwaggerWithAuthorization();
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddSingleton<MessagesService>()
    .AddScoped<ICurrentUser, CurrentUser>()
    ;
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

app.Run();