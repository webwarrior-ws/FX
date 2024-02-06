
using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;

namespace GrpcClient
{
    public enum ServerEnvironment
    {
        Local,
        Unknown,
        Production,
    }

    public class Instance
    {
        private static string serverFqdn =
            "localhost";

        public FXGrpcService.FXGrpcServiceClient Connect()
        {
            var channel = GrpcChannel.ForAddress($"http://{serverFqdn}:8080");
            var client = new FXGrpcService.FXGrpcServiceClient(channel);
            return client;
        }

        public static ServerEnvironment ServerEnvironment {
            get {
                if (serverFqdn == "localhost")
                {
                    return ServerEnvironment.Local;
                }
                else if (serverFqdn.EndsWith(".runinto.me"))
                {
                    return ServerEnvironment.Production;
                }
                else
                {
                    return ServerEnvironment.Unknown;
                }
            }
        }

        public async Task SendHello()
        {
            var client = Connect();
            var reply = await client.GenericMethodAsync(
                new GenericInputParam { MsgIn = "hello" }
            );
            Console.WriteLine($"Got response: {reply.MsgOut}");
        }
    }
}
