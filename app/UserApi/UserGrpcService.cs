using Grpc.Core;

namespace UserApi;

public class UserGrpcService: UserApi.UserApiBase
{
    public override Task<GetUserReply> GetUser(GetUserRequest request, ServerCallContext context)
    {
        return Task.FromResult(new GetUserReply()
        {
            User = new User()
            {
                Id = request.Id,
                Name = "gRPC name"
            }
        });
    }
}