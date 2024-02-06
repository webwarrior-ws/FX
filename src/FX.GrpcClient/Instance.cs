
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

        public static readonly int Port = 7178;

        public FXGrpcService.FXGrpcServiceClient Connect()
        {
            var channel = GrpcChannel.ForAddress($"https://{serverFqdn}:{Port}");
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

        public async Task<string> SendMessage(string message)
        {
            var client = Connect();
            var reply = await client.GenericMethodAsync(
                new GenericInputParam { MsgIn = message }
            );
            Console.WriteLine($"Got response: {reply.MsgOut}");
            return reply.MsgOut;
        }
    }
}
