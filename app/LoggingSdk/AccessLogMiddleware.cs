using System.Text;

namespace LoggingSdk;

public class AccessLogMiddleware(RequestDelegate next, ILogger<AccessLogMiddleware> logger)
{

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        request.EnableBuffering();
        var requestBodyText = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        
        logger.LogInformation("hello middleware {requestBody}", requestBodyText);
        await next(context);
    }
}