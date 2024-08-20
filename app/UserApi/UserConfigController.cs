using ConfigApi;
using Microsoft.AspNetCore.Mvc;

namespace UserApi;

public class UserConfigController(ConfigApi.ConfigApi.ConfigApiClient configApi, IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ConfigHttpApi");
    
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
    
    [HttpPatch("api/v1/user/{userId}/payment-config")]
    public async Task<List<PaymentConfig>> UpdateUserPayment([FromRoute] string userId, [FromBody] PaymentConfig paymentConfig)
    {
        var patchAsJsonAsync = await _httpClient.PatchAsJsonAsync("api/v1/payment-config", paymentConfig);

        return (await patchAsJsonAsync.Content.ReadFromJsonAsync<List<PaymentConfig>>())!;
    }
}

public class PaymentConfig
{
    public string PaymentMethod { get; set; }
    public decimal MaxAmount { get; set; }
}