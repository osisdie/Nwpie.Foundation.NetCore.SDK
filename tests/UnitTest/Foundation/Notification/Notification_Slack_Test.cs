using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Http.Common.Utilities;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Notification
{
    public class Notification_Slack_Test : TestBase
    {
        public Notification_Slack_Test(ITestOutputHelper output) : base(output) { }

        //[Fact(Skip = "TODO")]
        [Fact(Skip = "Skip this to save Slack quota")]
        public async Task Slack_Test()
        {
            var configValue = GetMapValueDefault(
               SysConfigKey.Default_Notification_Slack_Options_ConfigKey,
               EnvironmentEnum.Development.GetDisplayName()
            );
            Assert.NotNull(configValue);

            var slackCfg = Serializer.Deserialize<Slack_Option>(configValue);
            Assert.NotNull(slackCfg.DefaultChannel);
            Assert.NotNull(slackCfg.WebHookUrl);
            var msg = new
            {
                //channel = "#devops",
                channel = slackCfg.DefaultChannel,
                username = $"{ServiceContext.ApiName}-UnitTest",
                text = "Awesome !"
            };

            var jsonData = JsonConvert.SerializeObject(msg);
            var response = await ApiUtils.HttpPost(url: slackCfg.WebHookUrl,
                jsonData: jsonData,
                timeoutSecs: ConfigConst.DefaultHttpTimeout
            );

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
        }
    }
}
