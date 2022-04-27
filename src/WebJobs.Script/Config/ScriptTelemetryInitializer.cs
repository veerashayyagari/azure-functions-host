// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
            // clean up & set isenduserexception elsewhere
            if (FeatureFlags.IsEnabled(ScriptConstants.FeatureFlagEnableSurfaceCustomerExceptionToAI)
                && telemetry is ExceptionTelemetry exceptionTelemetry
                && exceptionTelemetry.Exception.InnerException is RpcException rpcException
                && !rpcException.IsEndUserException)
            {
                var exception = exceptionTelemetry.Exception;
                // only send relevant piece of stack trace to AI
                // exceptionTelemetry.SetParsedStack()
                telemetryProps["test_property_render"] = exception.InnerException.Message;
                //exceptionTelemetry.st
            }
        }
    }
}
