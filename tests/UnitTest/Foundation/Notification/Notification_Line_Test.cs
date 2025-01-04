using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Http.Common.Utilities;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Notification
{
    public class Notification_Line_Test : TestBase
    {
        public Notification_Line_Test(ITestOutputHelper output) : base(output) { }

        //[Fact(Skip = "TODO")]
        [Fact(Skip = "Skip this to save LINE monthly quota")]
        public async Task Line_Test()
        {
            var msg = new
            {
                to = "R40feac12ee7856541a53b83f13f35e6c",
                messages = new[] {
                    new
                    {
                        type = "text",
                        text = $"{ServiceContext.ApiName}-UnitTest-{"Awesome !"}"
                    }
                }
            };

            var jsonData = JsonConvert.SerializeObject(msg);
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { CommonConst.AuthHeaderName, "n61toeKKGxG3zJH4quqotdG4S6tnoWiNJLm1qZopfKbTl1q0JIkjMTEfLYiUO05wdy8ngvAW2ejuGX/MqjGOAsPrtcrnbSnvxFyCNdM4m2IqhZHJydqnLd10VlZ9DcGjDLz3dN6fvG1WEypm2cC1eAdB04t89/1O/w1cDnyilFU=".AttachBearer() }
            };

            var response = await ApiUtils.HttpPost(url: "https://api.line.me/v2/bot/message/push",
                jsonData: jsonData,
                headers: headers,
                timeoutSecs: ConfigConst.DefaultHttpTimeout
            );

            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
        }
    }
}
