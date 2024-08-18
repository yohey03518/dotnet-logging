using System.Net;
using LoggingSdk;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Events;
using UserApi;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Level:u}] {Timestamp:yyyy/MM/dd-HH:mm:ss} [{RequestId}] [{SourceContext}]{NewLine}{Message}{NewLine}{Exception}")
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();
builder.Services.AddGrpc(x => x.Interceptors.Add<GrpcLoggingInterceptor>());
builder.Services.AddGrpcReflection();
// builder.Services.AddHttpLogging(c =>
// {
//     c.LoggingFields = HttpLoggingFields.All;
//     c.CombineLogs = true;
// });

builder.Services.AddControllers(o =>
{
    o.Filters.Add<AccessLogFilter>();
});

builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.Listen(IPAddress.Any,  53664, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
    options.Listen(IPAddress.Any,  53665, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

builder.Services.AddSwaggerGen();

var app = builder.Build();
// app.UseHttpLogging();
app.UseMiddleware<AccessLogMiddleware>();

app.MapControllers();
app.Map("/ping", httpContext =>
{
    var log = httpContext.RequestServices.GetService<ILogger<UserInfoController>>();
    Log.Information("static");
    log.LogInformation("hello");
    return httpContext.Response.WriteAsync("pong");
});

app.UseRouting();
app.MapGrpcService<UserGrpcService>();
app.MapGrpcReflectionService();

app.Run();
