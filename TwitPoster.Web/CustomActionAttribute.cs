using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Serilog.Core;

namespace TwitPoster.Web;

public class CustomActionAttribute: ActionFilterAttribute
{
    private readonly ILogger<CustomActionAttribute> _logger;

    public CustomActionAttribute(ILogger<CustomActionAttribute> logger)
    {
        _logger = logger;
    }

    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogError("OnActionExecutionAsync");
        return base.OnActionExecutionAsync(context, next);
    }
}

public class CustomExceptionFilterAttribute: IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilterAttribute> _logger;

    public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError("Exception thrown EQWEQWRQWRQWRQWRQWAAAAAAAAAA");
    }
}