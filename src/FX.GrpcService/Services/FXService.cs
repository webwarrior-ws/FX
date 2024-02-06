using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
//using Microsoft.FSharp.Core;

using Grpc.Core;

namespace GrpcService.Services
{
    public class FXService : FXGrpcService.FXGrpcServiceBase
    {
        // Capacity of 5 is set here as a limit on how many unsent
        // notifications can the channel hold before it starts dropping
        // old ones. I don't think this ever happens but it's better
        // than having an unbounded channel.
        private const int notificationChannelCapacity = 5;
        
        private readonly ILogger<FXService> logger;

        private const int MaximumDistanceForSendingNotificationsInMeters = 300;
        private readonly TimeSpan MaxTimeDifferenceToConsiderFolksStillNearby = TimeSpan.FromMinutes(20);

        public FXService(ILogger<FXService> logger, IConfiguration configuration)
        {
            this.logger = logger;
        }

        public override async Task<GenericOutputParam> GenericMethod(GenericInputParam request, ServerCallContext context)
        {
            Console.WriteLine($"Received {request.MsgIn}");

            return new GenericOutputParam { MsgOut = "received " + request.MsgIn };
        }

        public override async Task GenericStreamOutputMethod(GenericInputParam request, IServerStreamWriter<GenericOutputParam> responseStream, ServerCallContext context)
        {
            Console.WriteLine(request.MsgIn);

            await responseStream.WriteAsync(new GenericOutputParam { MsgOut = "received " + request.MsgIn });
        }
    }
}
