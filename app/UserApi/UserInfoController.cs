using LoggingSdk;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace UserApi;

// [TypeFilter<AccessLogFilter>]
public class UserInfoController(ILogger<UserInfoController> logger): ControllerBase
{
    [HttpGet("api/v1/user")]
    public int Get(int id)
    {
        Log.Information($"static method log {id}");
        logger.LogInformation($"DI log {id}");

        return id;
    }
    
    [HttpGet("api/v2/user")]
    public JsonResult GetJson(int id)
    {
        return new JsonResult(new { id });
    }
}