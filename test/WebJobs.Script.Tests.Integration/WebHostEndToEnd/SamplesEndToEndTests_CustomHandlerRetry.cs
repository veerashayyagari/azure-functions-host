 using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.Tests.EndToEnd;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Integration.WebHostEndToEnd
{
    public class SamplesEndToEndTests_CustomHandlerRetry : IClassFixture<SamplesEndToEndTests_CustomHandlerRetry.TestFixture>
    {
        private readonly ScriptSettingsManager _settingsManager;
        private TestFixture _fixture;


        public SamplesEndToEndTests_CustomHandlerRetry(TestFixture fixture)
        {
            _fixture = fixture;
            _settingsManager = ScriptSettingsManager.Instance;
        }

        [Fact]
        public async Task CustomHandlerRetry_Get_Succeeds()
        {
            var response = await InvokeHttpTriggerCustomHandlerRetry("HttpTrigger");
            //var response = await SamplesTestHelpers.InvokeHttpTrigger(_fixture, "HttpTrigger");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string body = await response.Content.ReadAsStringAsync();
            JObject res = JObject.Parse(body);
            Assert.True(res["functionName"].ToString().StartsWith($"api/HttpTrigger"));
            Assert.Equal(res["retryCount"], "2");
        }

        public class TestFixture : EndToEndTestFixture
        {
            public TestFixture()
                :    base(Path.Combine(Environment.CurrentDirectory,"..", "..", "..", "..", "..","sample","CustomHandlerRetry"), "samples", RpcWorkerConstants.NodeLanguageWorkerName)
            {
            }

            public override void ConfigureScriptHost(IWebJobsBuilder webJobsBuilder)
            {
                base.ConfigureScriptHost(webJobsBuilder);
            }
        }
        private async Task<HttpResponseMessage> InvokeHttpTriggerCustomHandlerRetry(string functionName)
        {
            string functionKey = await _fixture.Host.GetFunctionSecretAsync($"{functionName}");
            string uri = $"api/{functionName}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            return await _fixture.Host.HttpClient.SendAsync(request);
        }
    }
}
