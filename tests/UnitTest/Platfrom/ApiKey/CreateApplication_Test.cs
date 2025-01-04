using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Platfrom.ApiKey
{
    public class CreateApplication_Test : TestBase
    {
        public CreateApplication_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test remote config server")]
        public void Admin_ApiKey_Test()
        {
            //foreach (var env in AdminApiKeyList)
            //foreach (var env in AdminApiKeyList.TakeLast(2))
            //foreach (var env in AdminApiKeyList.Take(3))
            foreach (var env in AdminApiKeyList)
            {
                var configuration = env.Key.ToString().ToLower();
                var e = Enum<EnvironmentEnum>.TryParseFromDisplayAttr(configuration, EnvironmentEnum.Max);
                Assert.NotEqual(EnvironmentEnum.Max, e);
            }
        }

        [Fact(Skip = "Won't test remote config server")]
        public async Task CreateApp_ByAuthService_Test()
        {
            var sb = new StringBuilder();
            sb.Append($"Created at: {DateTime.Now:s}{Environment.NewLine}");
            var apiKeyPrefix = "dropme";
            var note = "UnitTest";
            var filename = $"apikey_{apiKeyPrefix}.txt";

            //foreach (var env in AdminApiKeyList)
            //foreach (var env in AdminApiKeyList.TakeLast(2))
            //foreach (var env in AdminApiKeyList.Take(3))
            foreach (var env in AdminApiKeyList.Where(o => o.Key == "dev"))
            {
                var authServerUrl = AuthServiceUrlMap[env.Key.ToString()];
                Assert.NotNull(authServerUrl);

                //var request = new ContractRequest_New()
                //{
                //    Data = new ModelRequest_New()
                //    {
                //        ApiName = $"{apiKeyPrefix}.{env.Key}".ToLower(),
                //        Description = $"{note} for {env.Key}",
                //    }
                //};

                //var response = await m_RemoteConfigKeyClient.New(request);
                //Assert.True(response.IsSuccess);
                //Assert.False(string.IsNullOrWhiteSpace(response.Data.ApiKey));
                //Console.WriteLine(response.Data.ApiKey);
                //sb.Append($"{Environment.NewLine}###[{request.Data.ApiName}]{Environment.NewLine}");
                //sb.Append($"Application ApiKey:{response.Data.ApiKey}");
                //sb.Append($"{Environment.NewLine}```{Environment.NewLine}");
            }

            await File.WriteAllTextAsync(
                Path.Combine($"/{ConfigConst.DefaultTempFolder}",filename),
                sb.ToString());

            Assert.True(
                File.Exists(
                    Path.Combine($"/{ConfigConst.DefaultTempFolder}", filename)));
        }

        public override Task<bool> IsReady()
        {
            if (null == m_RemoteConfigKeyClient)
            {
                //m_RemoteConfigKeyClient = ComponentMgr.Instance.TryResolve<IClient_ApiKey>();
            }

            Assert.NotNull(m_RemoteConfigKeyClient);
            Assert.NotEmpty(AdminApiKeyList);

            return base.IsReady();
        }

        protected object m_RemoteConfigKeyClient;
    }
}
