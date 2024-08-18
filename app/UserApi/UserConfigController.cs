using ConfigApi;
using Microsoft.AspNetCore.Mvc;

namespace UserApi;

public class UserConfigController(ConfigApi.ConfigApi.ConfigApiClient configApi)
{
    [HttpGet("api/v1/user/{id}/config")]
    public async Task<UserConfig> GetUser(string id)
    {
        var result = await configApi.GetConfigAsync(new GetConfigRequest
        {
            Name = $"name{id}"
        });

        return new UserConfig()
        {
            Id = id,
            Name = result.Name,
            Value = result.Value
        };
    }
}