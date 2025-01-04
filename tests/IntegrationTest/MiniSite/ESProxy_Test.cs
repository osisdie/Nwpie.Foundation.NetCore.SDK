using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Auth.Contract.Login.SignIn;
using Nwpie.MiniSite.ES.Contract.Models;
using Newtonsoft.Json;
using ServiceStack;
using Xunit;

namespace Nwpie.IntegrationTest.MiniSite
{
    public class ESProxy_Test : TestBase
    {
        [Fact(Skip = "Won't test integration in unittests")]
        public async Task Flow_Test()
        {
            await LoginAuth_Test();

            await Query_Test();
        }

        private async Task LoginAuth_Test()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new LgnSignIn_Request
                {
                    Data = new LgnSignIn_RequestModel
                    {
                        Email = "dev@kevinw.net",
                        Password = "**"
                    }
                };

                var url = string.Concat($"{m_AuthBaseUrl}", request.ToPostUrl());
                var content = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json"
                );

                var message = await httpClient.PostAsync(url, content);
                var input = await message.Content.ReadAsStringAsync();
                Assert.NotNull(input);

                var response = JsonConvert.DeserializeObject<LgnSignIn_Response>(input);
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.NotNull(response.Data?.AccessToken);

                m_AccessToken = response.Data.AccessToken;
            }
        }

        private async Task Query_Test()
        {
            var request = new
            {
                from = 0,
                size = 1,
                query = new
                {
                    term = new
                    {
                        _id = "186591_636986273807561747:latest"
                    }
                }
            };

            using (var httpClient = new HttpClient())
            {
                var url = string.Concat($"{m_ESProxyBaseUrl}",
                    "/ds1_item", // index
                    "/_search", // search keyword
                    $"?token={m_AccessToken}" // Authorization
                );

                var content = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json"
                );

                var message = await httpClient.PostAsync(url, content);
                var input = await message.Content.ReadAsStringAsync();
                Assert.NotNull(input);

                var response = JsonConvert.DeserializeObject<ESProxyResponse<Ds1SourceItem>>(input);
                Assert.NotNull(response);
                Assert.NotEmpty(response.hits?.hits);
                Assert.NotNull(response.hits.hits.First()._source.version);
            }
        }

        //private const string INDEX = "ds1_item";
        private readonly string m_AuthBaseUrl = "https://api-dev.kevinw.net/auth";
        private readonly string m_ESProxyBaseUrl = "https://api-dev.kevinw.net/es";

        private string m_AccessToken = string.Empty;
    }

}
