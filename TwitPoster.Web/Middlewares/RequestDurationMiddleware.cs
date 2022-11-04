using System.Diagnostics;

namespace TwitPoster.Web.Middlewares;

public class RequestDurationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestDurationMiddleware> _logger;

    public RequestDurationMiddleware(RequestDelegate next, ILogger<RequestDurationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sp = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            var elapsedMilliseconds = sp.ElapsedMilliseconds;

            if (elapsedMilliseconds > 300)
            {
                _logger.LogWarning("Request {Method} {Path} took {ElapsedMilliseconds}ms", context.Request.Method, context.Request.Path, elapsedMilliseconds);
            }
        }
    }
}
