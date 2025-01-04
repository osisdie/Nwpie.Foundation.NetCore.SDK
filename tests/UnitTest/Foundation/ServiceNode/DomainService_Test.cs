using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class DomainService_Test : TestBase
    {
        public DomainService_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task GetRequester_Test()
        {
            // Correct Request
            {
                var request = new EchoTest_Request()
                {
                    Data = new EchoTest_RequestModel()
                    {
                        RequestString = "123"
                    }
                };

                using (var service = new EchoTest_Service())
                {
                    var response = await service.Any(request);
                    Assert.True(response.IsSuccess);
                    Assert.Equal((int)StatusCodeEnum.Success, response.Code);
                    Assert.NotNull(response.Data);
                    Assert.Equal(request.Data.RequestString, response.Data.ResponseString);
                }
            }

            // with requester
            {
                IEchoTest_DomainService svc = new EchoTest_DomainService
                {
                    Requester = DefaultTestAccountId
                };
                var requester = svc.GetRequester();
                Assert.Equal(DefaultTestAccountId, requester);
            }

            // without requester
            {
                IEchoTest_DomainService svc = new EchoTest_DomainService();
                var requester = svc.GetRequester();
                Assert.Null(requester);
            }
        }
    }
}
