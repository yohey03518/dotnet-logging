using System.Net;
using ConfigApi;
using LoggingSdk;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Events;

var outputTemplate = "[{Level:u}] {Timestamp:O} [{RequestId}] [{SourceContext}] {NewLine}{Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.File("logs/config-api.log", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog();
builder.Services.AddGrpc(x => x.Interceptors.Add<GrpcLoggingInterceptor>());
builder.Services.AddGrpcReflection();
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.Listen(IPAddress.Any,  53666, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
    options.Listen(IPAddress.Any,  53667, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

var app = builder.Build();
app.UseMiddleware<UseIncomingRequestIdMiddleware>();
app.UseMiddleware<AccessLogMiddleware>();
app.MapControllers();
app.MapGrpcService<ConfigGrpcService>();
app.MapGrpcReflectionService();

app.Run();