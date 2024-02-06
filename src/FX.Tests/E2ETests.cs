using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FsharpExchangeDotNetStandard;

using NUnit.Framework;
using StackExchange.Redis;

namespace FsharpExchange.Tests
{
    [TestFixture]
    public class E2ETests
    {
        [Test]
        async public Task GRPCE2ETest()
        {
            var solutionDir = (new DirectoryInfo(Environment.CurrentDirectory)).Parent?.Parent?.Parent?.Parent?.Parent;
            var serviceExePath =
                Path.Join(solutionDir.FullName, "src", "FX.GrpcService", "bin", "Debug", "net8.0", "FX.GrpcService.exe");

            using (var serverProcess = Process.Start(serviceExePath, $"--urls https://localhost:{GrpcClient.Instance.Port}"))
            {
                await Task.Delay(1000);

                var client = new GrpcClient.Instance();
                client.Connect();

                var message = "hello";
                var response = await client.SendMessage(message);

                Assert.AreEqual("received " + message, response);
            }
        }
    }
}
