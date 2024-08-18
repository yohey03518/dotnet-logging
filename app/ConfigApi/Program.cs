using System.Net;
using ConfigApi;
using LoggingSdk;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Level:u}] {Timestamp:yyyy/MM/dd-HH:mm:ss} [{RequestId}] [{SourceContext}]{NewLine}{Message}{NewLine}{Exception}")
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog();
builder.Services.AddGrpc(x => x.Interceptors.Add<GrpcLoggingInterceptor>());
builder.Services.AddGrpcReflection();
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.Listen(IPAddress.Any,  53666, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
    options.Listen(IPAddress.Any,  53667, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

var app = builder.Build();
app.UseMiddleware<AccessLogMiddleware>();
app.MapGrpcService<ConfigGrpcService>();
app.MapGrpcReflectionService();

app.Run();