// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.Azure.WebJobs.Script.DependencyInjection
{
    public class ServiceResolutionLogChannel
    {
        private static readonly object Lck = new object();
        private Channel<ServiceResolutionInfo> channel = Channel.CreateUnbounded<ServiceResolutionInfo>();
        private static ServiceResolutionLogChannel instance = null;

        private ServiceResolutionLogChannel()
        {
        }

        public static ServiceResolutionLogChannel Instance
        {
            get
            {
                lock (Lck)
                {
                    if (instance == null)
                    {
                        instance = new ServiceResolutionLogChannel();
                    }
                    return instance;
                }
            }
        }

        public ChannelReader<ServiceResolutionInfo> LogStream => channel.Reader;

        public void Send(ServiceResolutionInfo value)
        {
            // not awaiting. Let the task run like fire & forget.
            channel.Writer.WriteAsync(value);
        }
    }
}
