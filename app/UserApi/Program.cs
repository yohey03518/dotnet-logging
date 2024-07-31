using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.Listen(IPAddress.Any,  53664, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});

var app = builder.Build();

app.Map("/ping", httpContext => httpContext.Response.WriteAsync("pong"));
app.Run();
