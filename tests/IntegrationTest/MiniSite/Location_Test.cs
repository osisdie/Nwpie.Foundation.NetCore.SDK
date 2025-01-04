using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Newtonsoft.Json;
using ServiceStack;
using Xunit;

namespace Nwpie.IntegrationTest.MiniSite
{
    public class Location_Test : TestBase
    {
        [Fact(Skip = "Won't test integration in unittests")]
        public async Task Flow_Test()
        {
            await Query_Test();
        }

        async Task Query_Test()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add(CommonConst.ApiName, m_ClientApiName);
                httpClient.DefaultRequestHeaders.Add(CommonConst.ApiKey, m_ClientApiKey);

                var request = new LocGetApiLocation_Request()
                {
                    ApiKey = m_ClientApiKey,
                    AppName = m_FindAppName,
                    EnvInfo = new EnvInfo()
                    {
                        Env = m_ClientApiEnv,
                        IP = NetworkUtils.IP
                    }
                };

                var url = string.Concat($"{m_LocationBaseUrl}", request.ToPostUrl());
                var content = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json"
                );

                var message = await httpClient.PostAsync(url, content);
                var response = await message.Content.ReadAsStringAsync();
                Assert.False(string.IsNullOrWhiteSpace(response));
            }
        }

        private readonly string m_LocationBaseUrl = "https://api-dev.kevinw.net/loc";
        private readonly string m_FindAppName = "ds1-directly"; // deployment name
        private readonly string m_ClientApiEnv = "dev";
        private readonly string m_ClientApiName = "todo.dev";
        private readonly string m_ClientApiKey = "fake_api_key.debug";
    }
}
