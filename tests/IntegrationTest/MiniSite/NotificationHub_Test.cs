using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Notification.Contract.Notification;
using Newtonsoft.Json;
using ServiceStack;
using Xunit;

namespace Nwpie.IntegrationTest.MiniSite
{
    public class NotificationHub_Test : TestBase
    {
        [Fact(Skip = "Won't test integration in unittests")]
        public async Task Flow_Test()
        {
            await Query_Test();
        }

        private async Task Query_Test()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add(CommonConst.ApiName, m_ClientApiName);
                httpClient.DefaultRequestHeaders.Add(CommonConst.ApiKey, m_ClientApiKey);

                var request = new NtfySend_Request()
                {
                    Data = new NotifySend_RequestModel
                    {
                        Kind = (byte)NotifyChannelEnum.Email,
                        ToList = "dev@kevinw.net",
                        Title = $"{ServiceContext.SdkEnv}-{ServiceContext.ApiName}-Integration Test",
                        Message = "Awesome !"
                    }
                };
                var url = string.Concat($"{m_NotificationBaseUrl}", request.ToPostUrl());
                var content = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json"
                );

                var message = await httpClient.PostAsync(url, content);
                var input = await message.Content.ReadAsStringAsync();
                Assert.NotNull(input);

                var response = JsonConvert.DeserializeObject<NtfySend_Response>(input);
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
            }
        }

        private readonly string m_NotificationBaseUrl = "https://api-dev.kevinw.net/hub";
        //private readonly string m_FindAppName = "ds1-directly"; // deployment name
        //private readonly string m_ClientApiEnv = "dev";
        private readonly string m_ClientApiName = "todo.dev";
        private readonly string m_ClientApiKey = "fake_api_key.debug";
    }
}
