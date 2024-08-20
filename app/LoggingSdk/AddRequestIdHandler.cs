namespace LoggingSdk;

public class AddRequestIdHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContextTraceIdentifier = httpContextAccessor.HttpContext!.TraceIdentifier;
        request.Headers.Add(Constants.RequestIdHeader, httpContextTraceIdentifier);
        return await base.SendAsync(request, cancellationToken);
    }
}