// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.DependencyInjection
{
    public sealed class ServiceResolutionTrackerService : IHostedService
    {
        private readonly ILogger<ServiceResolutionTrackerService> _logger;

        private string filePath = string.Empty;

        private bool _shouldLogToTextFile = false;

        public ServiceResolutionTrackerService(ILogger<ServiceResolutionTrackerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _shouldLogToTextFile = string.Equals(Environment.GetEnvironmentVariable("FUNCTIONS_SERVICE_RESOLUTION_FILE_LOG_ENABLED"), "1");
        }

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
            _logger.LogInformation($"{nameof(ServiceResolutionTrackerService)} started listening");
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
            if (!_shouldLogToTextFile)
            {
                return;
            }

            string pathString = string.Empty;
            try
            {
                var path = Directory.GetCurrentDirectory();
                pathString = Path.Combine(path, "service-resolution-logs-" + Guid.NewGuid() + ".txt");

                if (!File.Exists(pathString))
                {
                    using (FileStream fs = File.Create(pathString))
                    {
                    }
                    filePath = pathString;
                    Console.WriteLine($"File {filePath} created");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating DI resolution log file. file path:{pathString}");
            }
        }

        private void WriteToLog(string line)
        {
            _logger.LogInformation(line);

            try
            {
                if (_shouldLogToTextFile)
                {
                    File.AppendAllLines(filePath, new string[] { line });
                }
            }
            catch (Exception)
            {
                // do nothing. We are already logging via ILogger.
            }
        }
    }
}