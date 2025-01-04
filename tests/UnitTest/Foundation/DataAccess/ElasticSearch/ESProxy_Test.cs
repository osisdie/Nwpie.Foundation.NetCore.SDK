using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.MiniSite.ES.Contract;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.ElasticSearch
{
    public class ESProxy_Test : TestBase
    {
        public ESProxy_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ReplaceHost_Test()
        {
            var frontendPostUrl = string.Concat(
                $"/{ESProxyServiceConfig.SysName}", // ingress
                "/ds1_item", // index
                "/_search"); // search keyword
            var proxyUrl = "elasticSearchBaseUrl"
                .ConfigServerRawValue();
            Assert.NotNull(proxyUrl);

            var replacedUrl = frontendPostUrl.Replace(
                $"/{ESProxyServiceConfig.SysName}/",
                $"{proxyUrl.TrimEndSlash()}/");
            Assert.Equal(
                $"{proxyUrl.TrimEndSlash()}/ds1_item/_search",
                replacedUrl);
        }
    }
}
