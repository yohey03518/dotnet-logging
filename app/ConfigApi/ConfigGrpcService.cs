using Grpc.Core;

namespace ConfigApi;

public class ConfigGrpcService: ConfigApi.ConfigApiBase
{
    public override Task<GetConfigReply> GetConfig(GetConfigRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetConfigReply
        {
            Name = request.Name,
            Value = "100"
        });
    }
}