using Microsoft.AspNetCore.Mvc;

namespace UserApi;

// [TypeFilter<AccessLogFilter>]
public class UserInfoController(ILogger<UserInfoController> logger): ControllerBase
{
    [HttpGet("api/v1/user")]
    public int Get(int id)
    {
        logger.LogInformation($"Logger from DI log {id}");
        return id;
    }

    [HttpPost("api/v1/user")]
    public MyUser Add([FromForm] MyUser user)
    {
        user.Id = 1;
        return user;
    }
}