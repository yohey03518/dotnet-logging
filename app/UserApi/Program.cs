using System.Net;
using LoggingSdk;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using UserApi;

var outputTemplate = "[{Level:u}] {Timestamp:O} [{RequestId}] [{SourceContext}] {NewLine}{Message}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.File("logs/user-api.log", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
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
    // o.Filters.Add<AccessLogFilter>();
});

builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.Listen(IPAddress.Any,  53664, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
    options.Listen(IPAddress.Any,  53665, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

builder.Services.AddSwaggerGen();
builder.Services.AddTransient<GrpcLoggingInterceptor>();
builder.Services.AddTransient<LogHttpMessageHandler>();
builder.Services.AddGrpcClient<ConfigApi.ConfigApi.ConfigApiClient>(x => x.Address = new Uri("http://localhost:53667"))
    .AddInterceptor<GrpcLoggingInterceptor>();

builder.Services.AddHttpClient("ConfigHttpApi", client => client.BaseAddress = new Uri("http://localhost:53666"))
    .AddHttpMessageHandler<LogHttpMessageHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<UserManagementDbContext>(o =>
{
    o.UseSqlServer("Data Source=127.0.0.1;Database=UserManagement;User ID=sa;Password=YourStrongPassword!123;TrustServerCertificate=True;");
    o.UseLoggerFactory(LoggerFactory.Create(c => c.AddSerilog()));
});

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
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();
}

app.Run();
