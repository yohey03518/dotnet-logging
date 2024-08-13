using Microsoft.Extensions.Logging;

namespace LoggingSdk;

public class AccessLogMiddleware(RequestDelegate next, ILogger<AccessLogMiddleware> logger)
{

    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation("hello middleware");
        await next(context);
    }
}