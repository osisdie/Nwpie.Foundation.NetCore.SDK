using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Nwpie.MiniSite.KVS.Contract;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class Minisite_KVS_Test : TestBase
    {
        public Minisite_KVS_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote service")]
        public async Task EchoRequest_http_GetData123_Test()
        {
            using (var client = new JsonServiceClient("http://localhost:5100"))
            {
                var response = await client.PostAsync<EchoTest_Response>(m_Request);
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.Equal(EchoString, response.Data?.ResponseString);
            }
        }

        [Fact(Skip = "Won't test remote service")]
        public async Task EchoRequest_ByLocation_GetData123_Test()
        {
            // By location
            {
                var response = await m_Request.InvokeAsyncByServiceName<EchoTest_Response>(
                    serviceName: KVServiceConfig.ServiceName
                );
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.Equal(EchoString, response.Data?.ResponseString);
            }
        }

        [Fact(Skip = "Won't test remote service")]
        public async Task EchoRequest_ViaSDK_GetData123_Test()
        {
            // By base url
            {
                var response = await m_Request.InvokeAsyncByBaseUrl<EchoTest_Response>(
                    baseUrl: "http://localhost:5100"
                );
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.Equal(EchoString, response.Data?.ResponseString);
            }

            // By absolote url
            {
                var response = await m_Request.InvokeAsyncByAbsoleteUrl<EchoTest_Response>(
                    "http://localhost:5100/HealthCheck/HlckEchoRequest"
                );
                Assert.NotNull(response);
                Assert.True(response.IsSuccess);
                Assert.Equal(EchoString, response.Data?.ResponseString);
            }
        }

        public override Task<bool> IsReady()
        {
            m_Request = new EchoTest_Request()
            {
                Data = new EchoTest_RequestModel()
                {
                    RequestString = EchoString
                }
            };

            return base.IsReady();
        }

        private const string EchoString = "123";

        protected EchoTest_Request m_Request;
    }
}
