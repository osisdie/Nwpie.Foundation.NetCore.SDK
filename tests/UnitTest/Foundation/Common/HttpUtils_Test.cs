using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Transform;
using Nwpie.Foundation.Http.Common.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Common
{
    public class HttpUtils_Test : TestBase
    {
        public HttpUtils_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task AddressDeserialize_Test()
        {
            var rawUrl = "https://raw.githubusercontent.com/donma/TaiwanAddressCityAreaRoadChineseEnglishJSON/master/AllData.json";
            var getResult = await ApiUtils.HttpGet(url: rawUrl,
                headers: null,
                timeoutSecs: 30
            );

            Assert.True(getResult.IsSuccess);
            Assert.NotNull(getResult.Data);
        }

        [Fact]
        public async Task ApiUtils_HttpPost_Test()
        {
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json" },
                {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0" },
            };

            var jsonData = new
            {
                Data = new
                {
                    ItemInfos = new Dictionary<string, string>
                    {
                        { "Id", "3228-12" }
                    }
                }
            };
            var rawUrl = "https://reqbin.com/echo/post/json";
            var result = await ApiUtils.HttpPost(url: rawUrl, jsonData: Newtonsoft.Json.JsonConvert.SerializeObject(jsonData), headers: headers, timeoutSecs: 30);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }
    }
}
