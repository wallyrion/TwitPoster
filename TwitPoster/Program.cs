using Microsoft.EntityFrameworkCore;
using Serilog;
using TwitPoster;
using TwitPoster.Extensions;
    
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lc) => lc
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

builder.Services.AddControllers();
builder.Services.AddFluentValidators();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuthorization();

builder.Services.AddDbContext<TwitPosterContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddJwtBearerAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();