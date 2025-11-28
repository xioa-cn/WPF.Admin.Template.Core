using External.GrpcServices.Utils;
using Grpc.Core;
using GrpcServices;
using WPF.Admin.Service.Services.Grpcs;

namespace External.GrpcServices.Services
{
    [GrpcService]
    public class NormalTestGrpcServerService : NormalTestGrpcServer.NormalTestGrpcServerBase
    {
        public override Task<TestResponse> Test(TestRequest request, ServerCallContext context)
        {
            Console.WriteLine($"gRPC服务收到请求: {request?.Request ?? "NULL"}");
            return Task.FromResult(new TestResponse { State = "OK" });
        }
    }
}