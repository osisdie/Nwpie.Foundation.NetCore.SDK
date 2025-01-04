using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Common.Config.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Auth
{
    public abstract class AuthTestBase : TestBase
    {
        public AuthTestBase(ITestOutputHelper output) : base(output) { }

        protected async Task<(TokenDataModel source, string encrypted)> GenerateTokenExpiresAfter(TimeSpan span, string acctId = "fake_user_id", string acctName = "unittest")
        {
            var now = DateTime.UtcNow;
            //var expiredInSpan = new TimeSpan(0, 0, 5);

            var source = new TokenDataModel()
            {
                AccountId = acctId,
                Name = acctName,
                Mob = false,
                Kind = (byte)TokenKindEnum.AccessToken,
                LV = (byte)TokenLevelEnum.EndUser,
                Iss = m_AuthClient.GetType().Name,
                Exp = now + span,
                Upt = now,
                Iat = now
            };

            var encrypted = await m_AuthClient.Encode(source);
            Assert.NotNull(encrypted);
            return (source, encrypted);
        }

        public override Task<bool> IsReady()
        {
            m_AuthOption = SysConfigKey
                .Default_Auth_ConfigKey
                .ConfigServerValue<Auth_Option>();
            Assert.NotNull(m_AuthOption);
            Assert.NotNull(m_AuthOption.JwtPrivateKey);

            m_AuthClient = new DefaultJwtAuthService<TokenDataModel>(
                new ConfigOptions<Auth_Option>(m_AuthOption),
                Serializer,
                cache: null
            );

            return base.IsReady();
        }

        protected Auth_Option m_AuthOption;
        protected ITokenService m_AuthClient;
    }
}
