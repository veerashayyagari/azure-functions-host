// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class ScriptTelemetryProcessor : ITelemetryProcessor
    {
        public ScriptTelemetryProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        private ITelemetryProcessor Next { get; set; }

        public void Process(ITelemetry item)
        {
            if (item is ExceptionTelemetry exceptionTelemetry && exceptionTelemetry.Exception is RpcException rpcException
                && rpcException.IsUserException)
            {
                exceptionTelemetry.Message = rpcException.RemoteMessage;

                // TODO - remove. For testing purposes while worker changes aren't in place yet.
                rpcException.RemoteTypeName = "test user exception type";

                string typeName = string.IsNullOrEmpty(rpcException.RemoteTypeName) ? rpcException.GetType().ToString() : rpcException.RemoteTypeName;

                var detailsInfoItem = exceptionTelemetry.ExceptionDetailsInfoList.FirstOrDefault(s => s.TypeName.Contains("RpcException"));
                exceptionTelemetry.ExceptionDetailsInfoList.FirstOrDefault(s => s.TypeName.Contains("RpcException")).TypeName = typeName;

                var userExceptionDetails = new ExceptionDetailsInfo(1, -1, typeName, rpcException.RemoteMessage, true, rpcException.RemoteStackTrace, new StackFrame[] { });

                ExceptionTelemetry newET = new ExceptionTelemetry(new[] { userExceptionDetails },
                SeverityLevel.Error, "ProblemId",
                new Dictionary<string, string>() { },
                new Dictionary<string, double>() { });
                this.Next.Process(newET);
            }
            else
            {
                this.Next.Process(item);
            }
        }
    }
}
