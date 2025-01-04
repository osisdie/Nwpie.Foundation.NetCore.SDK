using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Auth
{
    public class Apikey_Provider_Test : AuthTestBase
    {
        public Apikey_Provider_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ApiKey_Split_Test()
        {
            var apiName = "main.sub.debug";
            var sdkEnv = Utility.GetSDKEnvNameByApiName(apiName);
            Assert.Equal("debug", sdkEnv);

            var appName = Utility.GetSDKAppNameByApiName(apiName);
            Assert.Equal("main.sub", appName);
        }

        [Fact]
        public void Regex_ReplaceEnv_ReturnApiNameString()
        {
            var allPattern = AuthServiceConfig.MatchApiNamePattern;
            Assert.Matches(allPattern, "todo.base");
            Assert.Matches(allPattern, "todo.debug");
            Assert.Matches(allPattern, "todo.dev");
            Assert.Matches(allPattern, "todo.stage");
            Assert.Matches(allPattern, "todo.preprod");
            Assert.Matches(allPattern, "todo.prod");
            Assert.Matches(allPattern, "todo.child.debug");
            Assert.Matches(allPattern, "todo.child_2.debug");
            Assert.DoesNotMatch(allPattern, "todo.child-2.debug"); // not allow dash(-)

            // debug
            var currentPattern = AuthServiceConfig.MatchEnvApiNamePattern();
            Assert.Matches(currentPattern, "todo.base");
            Assert.Matches(currentPattern, "todo.debug");
            Assert.Matches(currentPattern, "todo.dev");
            Assert.Matches(currentPattern, "todo.stage");
            Assert.DoesNotMatch(currentPattern, "todo.preprod");
            Assert.DoesNotMatch(currentPattern, "todo.prod");
            Assert.Matches(currentPattern, "todo.child.debug");
            Assert.Matches(currentPattern, "todo.child_2.debug");
            Assert.DoesNotMatch(currentPattern, "todo.child-2.debug"); // not allow dash(-)

            var devPattern = AuthServiceConfig.MatchEnvApiNamePattern("dev");
            Assert.Matches(devPattern, "todo.base");
            Assert.Matches(devPattern, "todo.debug");
            Assert.Matches(devPattern, "todo.dev");
            Assert.Matches(devPattern, "todo.stage");
            Assert.DoesNotMatch(devPattern, "todo.preprod");
            Assert.DoesNotMatch(devPattern, "todo.prod");
            Assert.Matches(devPattern, "todo.child.debug");
            Assert.Matches(devPattern, "todo.child_2.debug");
            Assert.DoesNotMatch(devPattern, "todo.child-2.debug"); // not allow dash(-)

            var prodPattern = AuthServiceConfig.MatchEnvApiNamePattern("prod");
            Assert.Matches(prodPattern, "todo.base");
            Assert.DoesNotMatch(prodPattern, "todo.debug");
            Assert.DoesNotMatch(prodPattern, "todo.dev");
            Assert.DoesNotMatch(prodPattern, "todo.stage");
            Assert.Matches(prodPattern, "todo.preprod");
            Assert.Matches(prodPattern, "todo.prod");
            Assert.DoesNotMatch(prodPattern, "todo.prod.debug"); // not allow .debug
            Assert.DoesNotMatch(prodPattern, "todo.prod.debug"); // not allow .debug
            Assert.DoesNotMatch(prodPattern, "todo.prod-2.debug"); // not allow .debug, not allow dash(-)
        }

        [Fact]
        public async Task ApiKey_Token_Test()
        {
            var authOption = SysConfigKey
                .Default_Auth_ConfigKey
                .ConfigServerValue<Auth_Option>();
            Assert.NotNull(authOption);

            var client = new DefaultApiKeyAuthService<TokenDataModel>(
                new ConfigOptions<Auth_Option>(authOption),
                Serializer,
                cache: null
            );

            var now = DateTime.UtcNow;
            var expiredInSpan = new TimeSpan(0, 0, 10);

            var creator = $"{client.GetType().Name}{__}{authOption.MinimumApiTokenVersion}";
            var source = new TokenDataModel()
            {
                AccountId = "fake_app_id.debug",
                ApiKey = "fake_api_key.debug",
                Name = "todo",
                Mob = false,
                Kind = (byte)TokenKindEnum.AccessToken,
                LV = (byte)TokenLevelEnum.ApplicationUser,
                Iss = creator,
                Exp = now + expiredInSpan,
                Upt = now,
                Iat = now
            };

            var encrypted = await client.Encode(source);
            Assert.NotNull(encrypted);

            {
                var isAdmin = await client.IsAdmin(encrypted);
                Assert.False(isAdmin);

                var isValid = await client.IsGeneratedFromAuthServer(encrypted);
                Assert.True(isValid);

                var isExpired = await client.IsExpired(encrypted);
                Assert.False(isExpired);

                var decoded = await client.Decode(encrypted);
                Assert.False(string.IsNullOrWhiteSpace(decoded));

                var tokenModel = await client.Deserialize<TokenDataModel>(encrypted);
                Assert.NotNull(tokenModel);
                Assert.Equal(source.AccountId, tokenModel.AccountId);
                Assert.Equal(source.Name, tokenModel.Name);

                var verified = await client.VerifyToken<TokenDataModel>(encrypted,
                    TokenKindEnum.AccessToken
                );
                Assert.True(verified.IsSuccess);
                Assert.NotNull(verified.Data);
                Assert.Equal(source.AccountId, verified.Data.AccountId);
                Assert.Equal(source.Name, verified.Data.Name);
            }

            // revoke refreshToken by change version
            {
                await client.IncreseMinimumRefershTokenVersion();
                encrypted = await client.Encode(source);
                var isExpired = await client.IsExpired(encrypted);
                Assert.True(isExpired);
            }
        }
    }
}
