using System.Diagnostics;

namespace LoggingSdk;

public class LogHttpMessageHandler(ILogger<LogHttpMessageHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await LogRequest(request, cancellationToken);

        var stopWatch = Stopwatch.StartNew();
        var httpResponseMessage = await base.SendAsync(request, cancellationToken);
        stopWatch.Stop();

        await LogResponse(request, httpResponseMessage, stopWatch.ElapsedMilliseconds);
        return httpResponseMessage;
    }

    private async Task LogRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestContent = request.Content != null
            ? await request.Content.ReadAsStringAsync(cancellationToken)
            : string.Empty;

        logger.LogInformation("Request Start, Url: {Url} Method: {RequestMethod} RequestContent: {RequestContent}"
            , request.RequestUri!.AbsoluteUri, request.Method, requestContent);
    }

    private async Task LogResponse(HttpRequestMessage request, HttpResponseMessage httpResponseMessage, long elapsedMilliseconds)
    {
        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

        logger.LogInformation("Request End Url: {Url} Duration: {Duration}ms ResponseStatus: {StatusCode}\nResponseContent: {ResponseContent}"
            , request.RequestUri!.AbsoluteUri, elapsedMilliseconds, (int)httpResponseMessage.StatusCode, responseContent);
    }
}