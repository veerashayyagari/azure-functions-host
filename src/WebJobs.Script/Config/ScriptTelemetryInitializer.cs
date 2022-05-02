// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    internal class ScriptTelemetryInitializer : ITelemetryInitializer
    {
        private readonly ScriptJobHostOptions _hostOptions;

        public ScriptTelemetryInitializer(IOptions<ScriptJobHostOptions> hostOptions)
        {
            if (hostOptions == null)
            {
                throw new ArgumentNullException(nameof(hostOptions));
            }

            if (hostOptions.Value == null)
            {
                throw new ArgumentNullException(nameof(hostOptions.Value));
            }

            _hostOptions = hostOptions.Value;
        }

        public void Initialize(ITelemetry telemetry)
        {
            IDictionary<string, string> telemetryProps = telemetry?.Context?.Properties;

            if (telemetryProps == null)
            {
                return;
            }

            telemetryProps[ScriptConstants.LogPropertyHostInstanceIdKey] = _hostOptions.InstanceId;

            if (telemetry is ExceptionTelemetry exceptionTelemetry && exceptionTelemetry.Exception.InnerException is RpcException rpcException
                && rpcException.IsUserException && FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagEnableUserException))
            {
                exceptionTelemetry.Message = rpcException.RemoteMessage;
                string typeName = string.IsNullOrEmpty(rpcException.RemoteTypeName) ? rpcException.GetType().ToString() : rpcException.RemoteTypeName;
                exceptionTelemetry.ExceptionDetailsInfoList.FirstOrDefault(s => s.TypeName.Contains("RpcException")).TypeName = typeName;
            }
        }
    }
}
