using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace UserApi;

public class UserInfoController(ILogger<UserInfoController> logger): ControllerBase
{
    [HttpGet("api/v1/user")]
    public int Get(int id)
    {
        Log.Information($"static method log {id}");
        logger.LogInformation($"DI log {id}");

        return id;
    }
}