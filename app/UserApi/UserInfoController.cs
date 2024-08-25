using LoggingSdk;
using Microsoft.AspNetCore.Mvc;

namespace UserApi;

// [TypeFilter<AccessLogFilter>]
public class UserInfoController(ILogger<UserInfoController> logger): ControllerBase
{
    [HttpGet("api/v1/user")]
    public int Get(int id)
    {
        logger.LogInformation("log {id}", id);
        return id;
    }

    [HttpPost("api/v1/user")]
    public MyUser Add([FromForm] MyUser user)
    {
        user.Id = 1;
        return user;
    }

    [HttpPatch("api/v1/user")]
    public MyUser UpdateMobile([FromBody] UpdateMobileRequest request)
    {
        logger.LogInformation("request: {@request}", request);
        
        return new MyUser
        {
            Id = request.UserId,
            Name = "test",
            Email = "user@email.com",
            Mobile = request.NewMobile
        };
    }
}

public class UpdateMobileRequest
{
    public int UserId { get; set; }
    public required string NewMobile { get; set; }
}