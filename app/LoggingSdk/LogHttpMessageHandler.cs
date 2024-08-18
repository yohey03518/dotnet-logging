using System.Diagnostics;

namespace LoggingSdk;

public class LogHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger<LogHttpMessageHandler> _logger;

    public LogHttpMessageHandler(ILogger<LogHttpMessageHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestContent = await GetRequestContent(request);
        var message = $"[Send Request Start] Url: {request.RequestUri.AbsoluteUri} Method: {request.Method} RequestContent: {requestContent}";
        _logger.LogInformation(message);

        var stopWatch = Stopwatch.StartNew();
        var httpResponseMessage = await base.SendAsync(request, cancellationToken);
        stopWatch.Stop();

        await LogResponse(request, httpResponseMessage, stopWatch.ElapsedMilliseconds);
        return httpResponseMessage;
    }

    private async Task<string> GetRequestContent(HttpRequestMessage request)
    {
        var requestContent = request.Content != null && !await IsContainsImagePart(request) 
            ? await request.Content.ReadAsStringAsync()
            : string.Empty;

        return requestContent;
    }
    
    private async Task<bool> IsContainsImagePart(HttpRequestMessage request)
    {
        if (request.Content is MultipartFormDataContent multipartContent)
        {
            foreach (var contentPart in multipartContent)
            {
                if (contentPart.Headers.ContentDisposition?.Name?.Trim('"').ToLower() == "image")
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task LogResponse(HttpRequestMessage request, HttpResponseMessage httpResponseMessage, long elapsedMilliseconds)
    {
        var responseContent = httpResponseMessage.Content != null
            ? await httpResponseMessage.Content.ReadAsStringAsync()
            : string.Empty;

        var responseMessage = $"[Send Request End] Url: {request.RequestUri.AbsoluteUri} Duration:{elapsedMilliseconds} ResponseStatus: {httpResponseMessage.StatusCode}({(int)httpResponseMessage.StatusCode})\nResponseContent: {responseContent}";
        _logger.LogInformation(responseMessage);
    }
}