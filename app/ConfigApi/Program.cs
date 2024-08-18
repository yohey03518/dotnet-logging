// See https://aka.ms/new-console-template for more information

using System.Net;
using ConfigApi;
using LoggingSdk;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddSerilog();
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