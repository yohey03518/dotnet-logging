using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace LoggingSdk;

public class GrpcLoggingInterceptor(ILogger<GrpcLoggingInterceptor> logger) : Interceptor
{
    // client logging
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Send gRPC Request Start [{method}] [{Request}]", context.Method.FullName, request.ToString());

        var stopwatch = Stopwatch.StartNew();
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            LogClientResponse(context.Method.FullName, call, stopwatch),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    // server logging
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Receive gRPC Request Start [{host}] [{method}] [{Request}]", context.Host, context.Method, request.ToString());
        var stopwatch = Stopwatch.StartNew();
        var response = await continuation(request, context);
        stopwatch.Stop();
        var milliseconds = stopwatch.ElapsedMilliseconds;
        logger.LogInformation("Receive gRPC Request End [{host}] [{method}] [{time}ms] [{Response}]", context.Host, context.Method, milliseconds, response.ToString());
        return response;
    }

    private async Task<TResponse> LogClientResponse<TResponse>(string fullName, AsyncUnaryCall<TResponse> call, Stopwatch stopwatch)
    {
        var response = await call;
        logger.LogInformation("Send gRPC Request End [{method}] [{time}ms] [{Response}]", fullName, stopwatch.ElapsedMilliseconds, response?.ToString() ?? string.Empty);
        return response;
    }
}