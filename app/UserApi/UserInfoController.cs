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

    [HttpPost("api/v1/user")]
    public MyUser Add([FromForm] MyUser user)
    {
        user.Id = 1;
        return user;
    }
}

public class MyUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
}