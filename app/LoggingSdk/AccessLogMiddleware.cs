using System.Diagnostics;

namespace LoggingSdk;

public class AccessLogMiddleware(RequestDelegate next, ILogger<AccessLogMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;
        var isGrpc = request.ContentType == "application/grpc";
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var requestStartTime = DateTime.Now;
        var requestBodyText = isGrpc ? string.Empty : await GetRequestBody(request);

        string responseBodyText;

        if (!isGrpc)
        {
            var originalBodyStream = context.Response.Body;
            await using var memoryStream = new MemoryStream();
            response.Body = memoryStream;
        
            await next(context);
        
            responseBodyText = await GetResponseBody(response);
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        else 
        {
            await next(context);
            responseBodyText = string.Empty;
        }

        logger.LogInformation("{RequestTime} {method} {scheme} {host} {Path} {QueryString} {StatusCode} {ElapsedMilliseconds}ms {ClientIP} {RequestHeaders} {RequestBody} {ResponseHeaders} {ResponseBody}",
            requestStartTime.ToString("O"),
            isGrpc ? "gRPC" : request.Method,
            request.Scheme,
            request.Host,
            request.Path,
            request.QueryString,
            response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            request.HttpContext.Connection.RemoteIpAddress!.ToString(),
            // string.Join(';', request.Headers.Where(x => x.Key != "Cookie").Select(x => $"{x.Key}:{x.Value}")),
            "request header",
            requestBodyText,
            // string.Join(';', response.Headers.Where(x => x.Key != "Cookie").Select(x => $"{x.Key}:{x.Value}")),
            "response header",
            isGrpc ? string.Empty : responseBodyText
        );
    }

    private static async Task<string> GetResponseBody(HttpResponse response)
    {
        response.Body.Position = 0;
        var responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();

        response.Body.Position = 0;
        return responseBodyText;
    }

    private static async Task<string> GetRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        var requestBodyText = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;
        return requestBodyText;
    }
}