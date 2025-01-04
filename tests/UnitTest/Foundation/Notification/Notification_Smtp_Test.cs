using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Notification.Smtp;
using Nwpie.xUnit.Resources;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Notification
{
    public class Notification_Smtp_Test : TestBase
    {
        public Notification_Smtp_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "TODO")]
        public void Title_Test()
        {
            const string title = "Title-UnitTest";
            var envTitle = title.SetTitlePrefixInDevelopment();

            if (ServiceContext.IsDebug)
            {
                Assert.Equal($"[debug] {title}", envTitle);
            }
            else if (ServiceContext.IsDevelopment())
            {
                Assert.Equal($"[dev] {title}", envTitle);
            }
            else
            {
                Assert.Equal(title, envTitle);
            }
        }

        [Fact(Skip = "TODO")]
        public async Task Email_Test()
        {
            var configValue = GetMapValueDefault(
                SysConfigKey.Default_Notification_Smtp_Options_ConfigKey,
                EnvironmentEnum.Development.GetDisplayName()
            );
            Assert.NotNull(configValue);

            var smtpCfg = new ConfigOptions<Smtp_Option>(
                Serializer.Deserialize<Smtp_Option>(configValue)
            );
            Assert.NotNull(smtpCfg.Value.Host);
            Assert.NotNull(smtpCfg.Value.Port);

            var mailer = new SmtpMailService(smtpCfg)
            {
                Template = Resource1.mail_template
            };
            mailer.AddReceiver(new List<string>() { "dev@kevinw.net" });
            mailer.Subject = $"{ServiceContext.ApiName}-UnitTest".SetTitlePrefixInDevelopment();
            mailer.Body = "Awesome !";

            // Not works
            mailer.Context.From = new MailAddress("no-reply@kevinw.net");

            // Receiver will see the list
            //  "no-reply@kevinw.net",
            //  "dev@kevinw.net"
            mailer.Context.ReplyToList.Clear();
            mailer.Context.ReplyToList.Add("no-reply@kevinw.net");

            using (mailer)
            {
                var result = await mailer.SendAsync();
                Assert.True(result.IsSuccess);
            }
        }
    }
}
