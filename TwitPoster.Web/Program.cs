using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using TwitPoster.DAL;
using TwitPoster.Web;
using TwitPoster.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lc) => lc
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

builder.Services.AddControllers();
builder.Services.AddFluentValidators();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithAuthorization();

builder.Services.AddDbContext<TwitPosterContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddJwtBearerAuthentication();

builder.Services.AddProblemDetails();

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

// app.UseExceptionHandler();
// app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
}


app.Run();