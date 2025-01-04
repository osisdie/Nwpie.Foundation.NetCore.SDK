using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.SDK.Providers;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Config.Extensions;
using Newtonsoft.Json.Linq;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Auth.SDK.Interfaces;

namespace Nwpie.xUnit.Foundation.Auth
{
    public class TokenDecode_Test : AuthTestBase
    {
        public TokenDecode_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Decode_Test()
        {
            var expiredInSpan = new TimeSpan(0, 0, 5);
            var acctId = "fake_user_id";
            var acctName = "unittest";
            var tokenized = await GenerateTokenExpiresAfter(expiredInSpan, acctId, acctName);
            var source = tokenized.source;
            var token = tokenized.encrypted;
            Assert.NotNull(token);

            var jwtPrivateKey = m_AuthOption.JwtPrivateKey;
            var verify = true; // signature

            try
            {
                var parts = token.Split('.');
                var header = parts[0];
                var payload = parts[1];
                var crypto = JwtUtils.Base64UrlDecode(parts[2]);

                var headerJson = Encoding.UTF8.GetString(JwtUtils.Base64UrlDecode(header));
                var headerData = JObject.Parse(headerJson);
                var payloadJson = Encoding.UTF8.GetString(JwtUtils.Base64UrlDecode(payload));
                var payloadData = JObject.Parse(payloadJson);

                if (verify)
                {
                    var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                    var keyBytes = Encoding.UTF8.GetBytes(jwtPrivateKey);
                    var algorithm = (string)headerData["alg"];

                    var signature = JwtUtils.HashAlgorithms[JwtUtils.GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                    var decodedCrypto = Convert.ToBase64String(crypto);
                    var decodedSignature = Convert.ToBase64String(signature);

                    Assert.Equal(decodedSignature, decodedCrypto);
                }
            }
            catch (Exception ex)
            {
                Assert.Null(ex.Message);
            }

            await Task.CompletedTask;
        }

        [Fact(Skip = "Already created")]
        public async Task Jwt_PrivateKey_Test()
        {
            var sb = new StringBuilder();
            sb.Append($"{DateTime.Now:s}{Environment.NewLine}");

            // All          => for env in AdminApiKeyList
            // Production   => for env in AdminApiKeyList.TakeProduction()
            // Development  => for env in AdminApiKeyList.TakeNonProduction()
            foreach (var env in AdminApiKeyList)
            {
                var privateKey = RsaUtils.CreatePrivateKeyParams(RsaKeyLengths.Bit2048);

                sb.Append($"{env.Key}:{Environment.NewLine}");
                sb.Append($"----------{Environment.NewLine}");
                var privateKeyXml = privateKey.ToPrivateKeyXml();
                sb.Append($"Privatekey:");
                sb.Append(Environment.NewLine);
                sb.Append(privateKeyXml);
                sb.Append(Environment.NewLine);
                var publicKeyXml = privateKey.ToPublicKeyXml();
                sb.Append($"Publickey:");
                sb.Append(Environment.NewLine);
                sb.Append(publicKeyXml);
                sb.Append(Environment.NewLine);
                var authKey = AesUtils.CreateKey();
                sb.Append($"AuthKeyBase64:");
                sb.Append(Environment.NewLine);
                sb.Append(Convert.ToBase64String(authKey));
                sb.Append(Environment.NewLine);
            }

            var fullpath = Path.Combine(
                $"/{ConfigConst.DefaultTempFolder}",
                "jwt.txt"
            );
            await File.WriteAllTextAsync(fullpath, sb.ToString());
            Assert.True(File.Exists(fullpath));
        }

        [Fact]
        public void Jwt_Decode_Test()
        {
            var createdAt = DateTime.UtcNow.AddHours(-4);
            var user = new TokenDataModel()
            {
                AccountId = "fake_user_id",
                Name = "unittest",
                Mob = false,
                Kind = (byte)TokenKindEnum.AccessToken,
                LV = (byte)TokenLevelEnum.EndUser,
                Iss = typeof(DefaultJwtAuthService<>).Name,
                Exp = DateTime.UtcNow,
                Upt = createdAt,
                Iat = createdAt,
            };

            var payload = Serializer.Serialize(user);
            var alg = JwtUtils.DefaultJwtAlgorithm;

            var authOption = SysConfigKey
                .Default_Auth_ConfigKey
                .ConfigServerValue<Auth_Option>();
            Assert.NotNull(authOption);
            Assert.NotNull(authOption.JwtPrivateKey);

            var str = JwtUtils.Encode(user, authOption.JwtPrivateKey, alg);
            var backToUser = JwtUtils.Decode<TokenDataModel>(str, authOption.JwtPrivateKey, true);
            Assert.True(backToUser.IsSuccess);
            Assert.NotNull(backToUser.Data);
            Assert.Equal(user.Name, backToUser.Data.Name);
        }

        public override Task<bool> IsReady()
        {
            Assert.NotEmpty(AdminApiKeyList);

            return base.IsReady();
        }
    }
}
