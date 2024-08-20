namespace LoggingSdk;

public class UseIncomingRequestIdMiddleware(RequestDelegate next, ILogger<UseIncomingRequestIdMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var requestId = context.Request.Headers[Constants.RequestIdHeader].ToString();

        if (string.IsNullOrWhiteSpace(requestId))
        {
            // no value, use default generate value
            await next(context);
        }
        else
        {
            context.TraceIdentifier = requestId;
            using (logger.BeginScope("{RequestId}", requestId))
            {
                await next(context);
            }
        }
    }
}