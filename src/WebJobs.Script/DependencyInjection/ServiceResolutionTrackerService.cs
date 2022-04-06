// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script.DependencyInjection
{
    internal class ServiceResolutionTrackerService : IHostedService
    {
        private string filePath = string.Empty;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = StartListening(cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task StartListening(CancellationToken ct)
        {
            Console.WriteLine("ServiceResolutionTrackerService Started Listening");
            CreateLogFile();
            ISourceBlock<ServiceResolutionInfo> source = ServiceResolutionLogChannel.Instance.LogStream;

            try
            {
                while (await source.OutputAvailableAsync(ct))
                {
                    var msg = await source.ReceiveAsync();
                    WriteToLog(msg.Name + ", duration:" + msg.TimeTaken.TotalMilliseconds + "(ms)");
                }
            }
            catch (OperationCanceledException)
            {
                // This occurs during shutdown.
            }
        }

        private void CreateLogFile()
        {
            string path = Directory.GetCurrentDirectory();
            string pathString = Path.Combine(path, "service-resolution-logs-" + Guid.NewGuid() + ".txt");

            if (!File.Exists(pathString))
            {
                using (FileStream fs = File.Create(pathString))
                {
                }
                filePath = pathString;
                Console.WriteLine($"File {filePath} created");
            }
        }

        private void WriteToLog(string line)
        {
            File.AppendAllLines(filePath, new string[] { line });
            //Console.WriteLine("Received in buffer block:" + line);
        }
    }
}
