// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.WebHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ScriptTelemetryProcessorTests
    {
        // TEMP TEST - TO REMOVE
        [Fact]
        public void Test_TelemetryProcessor_AppInsights()
        {
            var telemetry = new RequestTelemetry
            {
                Url = new Uri("https://localhost/api/function?name=World"),
                ResponseCode = "200",
                Name = "Function Request"
            };

            var jobHostOptions = new ScriptJobHostOptions();
            var hostInstanceID = jobHostOptions.InstanceId;


            var initializer = new ScriptTelemetryInitializer(new OptionsWrapper<ScriptJobHostOptions>(jobHostOptions));
            initializer.Initialize(telemetry);

            Assert.True(telemetry.Context.Properties.TryGetValue(ScriptConstants.LogPropertyHostInstanceIdKey, out string telemetryHostId));
            Assert.Equal(hostInstanceID, telemetryHostId);
        }

        private class TestScriptHostBuilder : IScriptHostBuilder
        {
            private readonly DefaultScriptHostBuilder _builder;

            public TestScriptHostBuilder(IOptionsMonitor<ScriptApplicationHostOptions> appHostOptions, IServiceProvider rootServiceProvider, IServiceScopeFactory rootServiceScopeFactory)
            {
                _builder = new DefaultScriptHostBuilder(appHostOptions, rootServiceProvider, rootServiceScopeFactory);
            }

            public IHost BuildHost(bool skipHostStartup, bool skipHostConfigurationParsing)
            {
                // always skip host startup
                return _builder.BuildHost(true, false);
              // _builder.Services.AddTelemetryProcessor();
            }
        }
    }
}
