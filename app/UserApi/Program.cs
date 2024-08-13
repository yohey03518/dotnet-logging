﻿using System.Net;
using LoggingSdk;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using UserApi;

Log.Logger = new LoggerConfiguration()
    // .WriteTo.File("mylog.txt")
    .WriteTo.Console(outputTemplate: "[{Level:u}] {Timestamp:O} [{RequestId}] - {Message}{NewLine}{Exception}")
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSerilog();

builder.Services.AddControllers(o =>
{
    o.Filters.Add<AccessLogFilter>();
});

builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.Listen(IPAddress.Any,  53664, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
});

var app = builder.Build();
app.UseMiddleware<AccessLogMiddleware>();
app.MapControllers();

app.Map("/ping", httpContext =>
{
    var log = httpContext.RequestServices.GetService<ILogger<UserInfoController>>();
    log.LogInformation("hello");
    return httpContext.Response.WriteAsync("pong");
});
app.Run();
