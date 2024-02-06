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
        private Process LaunchGrpcServer()
        {
            var solutionDir = (new DirectoryInfo(Environment.CurrentDirectory)).Parent?.Parent?.Parent?.Parent?.Parent;
            var serviceExeDir = Path.Join(solutionDir.FullName, "src", "FX.GrpcService", "bin", "Debug", "net8.0");

            var argsString = $"--urls https://localhost:{GrpcClient.Instance.Port}";

            if (OperatingSystem.IsWindows())
            {
                var serviceExePath = Path.Join(serviceExeDir, "FX.GrpcService.exe");
                return Process.Start(serviceExePath, argsString);
            }
            else 
            {
                var processInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = "dotnet",
                    Arguments = Path.Join(serviceExeDir, "FX.GrpcService.dll") + " " + argsString
                };
                return Process.Start(processInfo);
            }
        }

        [Test]
        async public Task GRPCE2ETest()
        {
            var solutionDir = (new DirectoryInfo(Environment.CurrentDirectory)).Parent?.Parent?.Parent?.Parent?.Parent;
            var serviceExeDir = Path.Join(solutionDir.FullName, "src", "FX.GrpcService", "bin", "Debug", "net8.0");

            using var serverProcess = LaunchGrpcServer();
            await Task.Delay(1000);

            var client = new GrpcClient.Instance();
            client.Connect();

            var message = "hello";
            var response = await client.SendMessage(message);

            Assert.AreEqual("received " + message, response);
        }
    }
}
