using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Auth
{
    public class GenerateAccountToken_Test : AuthTestBase
    {
        public GenerateAccountToken_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Already created")]
        public async Task ApiKeyAccount_TokenGenerate_Test()
        {
            var appList = ServiceContext.Configuration
                .GetSection("sdk.api.list")
                ?.GetChildren()
                ?.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)); ;
            Assert.NotEmpty(appList);

            var now = DateTime.UtcNow;
            var sb = new StringBuilder();
            sb.Append($"Created at: {now.Date:s}{Environment.NewLine}");

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var app in appList)
            {
                var apiName = app.Key.ToLower();
                var apiKey = app.Value;
                if (m_AppPrefixList?.Count > 0 && true != m_AppPrefixList?.Any(x => apiName.Contains(x)))
                {
                    continue;
                }

                var stringEnv = Utility.GetSDKEnvNameByApiName(apiName);
                var typedEnv = Enum<EnvironmentEnum>.TryParseFromDisplayAttr(stringEnv, EnvironmentEnum.Max);
                Assert.NotEqual(EnvironmentEnum.Max, typedEnv);
                //if (EnvironmentEnum.Development != Enum<EnvironmentEnum>.TryParseFromDisplayAttr(stringEnv, EnvironmentEnum.Max))
                //{
                //    continue;
                //}

                var authServerUrl = AuthServiceUrlMap[typedEnv.GetDisplayName()];
                Assert.NotNull(authServerUrl);
                TokenProfileMgr.Instance.AuthServiceUrl = authServerUrl;

                var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey,
                    senderApiName: apiName,
                    senderApiKey: apiKey
                );
                if (string.IsNullOrWhiteSpace(profile?.AccountId))
                {
                    continue;
                }

                var authConfigValue = GetMapValueDefault(SysConfigKey.Default_Auth_ConfigKey,
                    stringEnv,
                    EnvironmentEnum.Max
                );
                Assert.NotNull(authConfigValue);

                var authOption = new ConfigOptions<Auth_Option>(
                    Serializer.Deserialize<Auth_Option>(authConfigValue));

                var tokenService = new DefaultJwtAuthService<TokenDataModel>(
                    authOption,
                    Serializer,
                    cache: null
                );

                var tokenExpiresInDays = APP_TOKEN_DAYS;
                if (EnvironmentEnum.Production == typedEnv)
                {
                    tokenExpiresInDays = APP_TOKEN_DAYS / 2;
                }

                var expiredInSpan = TimeSpan.FromDays(tokenExpiresInDays);
                sb.Append($"Expired at: {now.Date.AddDays(tokenExpiresInDays):s}{Environment.NewLine}");
                sb.Append(Environment.NewLine);

                var creator = $"{tokenService.GetType().Name}{__}{authOption.Value.MinimumApiTokenVersion}";
                var tokenMdl = new TokenDataModel
                {
                    AppId = AUTH_APP_ID,
                    AccountId = profile.AccountId,
                    ApiKey = apiKey,
                    Name = profile.Name, //apiName,
                    Mob = false,
                    Kind = (byte)TokenKindEnum.AccessToken,
                    LV = (byte)TokenLevelEnum.ApplicationUser,
                    Iss = creator,
                    //Exp = now.Date + expiredInSpan,
                    Upt = now,
                    Iat = now
                };

                sb.Append($"{Environment.NewLine}###[{apiName}]{Environment.NewLine}");
                sb.Append($"Applicationi level token, algorithm:{authOption.Value.JwtAlgorithm}, apiName={apiName}, apiKey={apiKey}");
                sb.Append($"{Environment.NewLine}```{Environment.NewLine}");
                sb.Append($"-----BEGIN JWT TOKEN-----{Environment.NewLine}");

                var encoded = await tokenService.Encode(tokenMdl);
                Assert.NotNull(encoded);

                sb.Append(encoded);
                sb.Append($"{Environment.NewLine}-----END JWT TOKEN-----{Environment.NewLine}");
                sb.Append($"```{Environment.NewLine}");
            }

            var fullPath = Path.Combine(
                $"/{ConfigConst.DefaultTempFolder}",
                $"jwt_Apikey_AccessToken_{now:yyyyMMdd}_{now:HHmmss}_generated.txt"
            );
            await File.WriteAllTextAsync(fullPath, sb.ToString());
            Assert.True(File.Exists(fullPath));
        }

        [Fact(Skip = "Already created")]
        public async Task Account_TokenGenerate_Test()
        {
            Assert.NotEmpty(m_AccountList);

            var pattern = new Regex(@"^(assets\.app\.dev)$");
            var appList = ServiceContext.Configuration
                .GetKeyValueListInSection("sdk.api.list", pattern);
            Assert.NotEmpty(appList);

            var now = DateTime.UtcNow;
            var sb = new StringBuilder();
            sb.Append($"Created at: {now.Date:s}{Environment.NewLine}");
            sb.Append(Environment.NewLine);

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var app in appList)
            {
                var apiName = app.Key.ToLower();
                var apiKey = app.Value;
                //Assert.True(apiName.Contains(ConfigConst.ApiNameDivider));

                var stringEnv = Utility.GetSDKEnvNameByApiName(apiName);
                var typedEnv = Enum<EnvironmentEnum>.TryParseFromDisplayAttr(stringEnv, EnvironmentEnum.Max);
                Assert.NotEqual(EnvironmentEnum.Max, typedEnv);

                var authServerUrl = AuthServiceUrlMap[typedEnv.GetDisplayName()];
                Assert.NotNull(authServerUrl);
                TokenProfileMgr.Instance.AuthServiceUrl = authServerUrl;

                var authConfigValue = GetMapValueDefault(SysConfigKey.Default_Auth_ConfigKey,
                    stringEnv,
                    EnvironmentEnum.Max
                );
                Assert.NotNull(authConfigValue);

                var authOption = new ConfigOptions<Auth_Option>(Serializer.Deserialize<Auth_Option>(authConfigValue));
                var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey,
                    senderApiName: apiName,
                    senderApiKey: apiKey
                );
                if (string.IsNullOrWhiteSpace(profile?.AccountId))
                {
                    continue;
                }

                var tokenService = new DefaultJwtAuthService<TokenDataModel>(
                    authOption,
                    Serializer,
                    null
                );

                var tokenExpiresInDays = ACCOUNT_TOKEN_DAYS;
                if (EnvironmentEnum.Production == typedEnv)
                {
                    tokenExpiresInDays = ACCOUNT_TOKEN_DAYS;
                }

                var creator = $"{tokenService.GetType().Name}{__}{authOption.Value.MinimumApiTokenVersion}";
                var expiredInSpan = TimeSpan.FromDays(tokenExpiresInDays);
                sb.Append($"Expired at: {now.Date.AddDays(tokenExpiresInDays):s}{Environment.NewLine}");
                sb.Append(Environment.NewLine);

                foreach (var user in m_AccountList)
                {
                    user.AppId = AUTH_APP_ID;
                    user.AccountId = profile.AccountId;
                    user.Name = profile.Name;
                    user.Mob = false;
                    user.Kind = (byte)TokenKindEnum.AccessToken;
                    user.LV = (byte)TokenLevelEnum.EndUser;
                    user.Iss = creator;
                    user.Exp = now.Date + expiredInSpan;
                    user.Upt = now;
                    user.Iat = now;

                    sb.Append($"{Environment.NewLine}###[{user.Name}]{Environment.NewLine}");
                    sb.Append($"User level token, algorithm:{authOption.Value.JwtAlgorithm}, user={user.Name}");
                    sb.Append($"{Environment.NewLine}```{Environment.NewLine}");
                    sb.Append($"-----BEGIN JWT TOKEN-----{Environment.NewLine}");

                    var encoded = await tokenService.Encode(user);
                    Assert.NotNull(encoded);
                    sb.Append(encoded);
                    sb.Append($"{Environment.NewLine}-----END JWT TOKEN-----{Environment.NewLine}");
                    sb.Append($"```{Environment.NewLine}");
                }
            }

            var fullPath = Path.Combine(
                $"/{ConfigConst.DefaultTempFolder}",
                $"jwt_User_AccessToken_{now:yyyyMMdd}_{now:HHmmss}_generated.txt"
            );
            await File.WriteAllTextAsync(fullPath, sb.ToString());
            Assert.True(File.Exists(fullPath));
        }

        public override Task<bool> IsReady()
        {
            m_TokenService = new DefaultJwtAuthService<TokenDataModel>(
                new ConfigOptions<Auth_Option>(m_AuthOption),
                Serializer,
                cache: null
            );

            m_AuthHostUrlList = ServiceContext.Configuration
                .GetSection("sdk.auth.host_url")
                ?.GetChildren()
                ?.Select(x => new KeyValuePair<string, string>(x.Key, x.Value));
            Assert.NotEmpty(m_AuthHostUrlList);

            return base.IsReady();
        }

        protected readonly List<string> m_AppPrefixList = new List<string>()
        {
            "auth.service.prod",
            "sdk.admin.",
            "configserver.",
            "sdk.storage.",
            "ntfy.",
            "acct.",
            "tlda.",
            "asset.service.",
            "todo.",
            "auth.service.",
            "asset.forstore.",
        };

        protected readonly List<TokenDataModel> m_AccountList = new List<TokenDataModel>()
        {
            new TokenDataModel()
            {
                AccountId = "fake_hp_1",
                Name = "HP Demo1",
            },
            new TokenDataModel()
            {
                AccountId = "fake_hp_2",
                Name = "HP Demo2",
            },
            new TokenDataModel()
            {
                AccountId = "fake_hp_3",
                Name = "HP Demo3",
            },
            new TokenDataModel()
            {
                AccountId = "fake_hp_4",
                Name = "HP Demo4",
            },
            new TokenDataModel()
            {
                AccountId = "fake_hp_5",
                Name = "HP Demo5",
            },
        };

        private const string AUTH_APP_ID = "fake_app_id.debug";
        private const int APP_TOKEN_DAYS = 365;
        private const int ACCOUNT_TOKEN_DAYS = 90;

        private IEnumerable<KeyValuePair<string, string>> m_AuthHostUrlList;
        private DefaultJwtAuthService<TokenDataModel> m_TokenService;
    }
}
