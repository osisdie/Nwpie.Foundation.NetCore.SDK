using System;
using System.Threading;
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
    public class AES_Provider_Test : AuthTestBase
    {
        public AES_Provider_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task AES_AdminToken_Test()
        {
            {
                var isAdmin = await m_AuthClient.IsAdmin(m_AuthOption.AdminToken);
                Assert.True(isAdmin);

                var isValid = await m_AuthClient.IsGeneratedFromAuthServer(m_AuthOption.AdminToken);
                Assert.True(isValid);

                var isExpired = await m_AuthClient.IsExpired(m_AuthOption.AdminToken);
                Assert.False(isExpired);
            }

            var decoded = await m_AuthClient.Decode(m_AuthOption.AdminToken);
            Assert.False(string.IsNullOrWhiteSpace(decoded));

            var tokenModel = await m_AuthClient.Deserialize<TokenDataModel>(m_AuthOption.AdminToken);
            Assert.NotNull(tokenModel);
            Assert.NotNull(tokenModel.AccountId);
            Assert.NotNull(tokenModel.Name);
            Assert.Equal((byte)TokenLevelEnum.ApplicationUser, tokenModel.LV);

            var verified = await m_AuthClient.VerifyToken<TokenDataModel>(m_AuthOption.AdminToken,
                TokenKindEnum.AccessToken
            );
            Assert.True(verified.IsSuccess);
            Assert.NotNull(verified.Data);
            Assert.NotNull(verified.Data.AccountId);
            Assert.NotNull(verified.Data.Name);
            Assert.Equal((byte)TokenLevelEnum.ApplicationUser, tokenModel.LV);

            var renewed = await m_AuthClient.RenewToken(m_AuthOption.AdminToken);
            Assert.False(string.IsNullOrWhiteSpace(renewed));

            // Check renewed token
            {
                {
                    var isExpired = await m_AuthClient.IsExpired(renewed);
                    Assert.False(isExpired);
                }

                await m_AuthClient.IncreseMinimumRefershTokenVersion();

                {
                    var isExpired = await m_AuthClient.IsExpired(renewed);
                    Assert.False(isExpired);
                }
            }
        }

        [Fact]
        public async Task AES_UserToken_Test()
        {
            var now = DateTime.UtcNow;
            var expiredInSpan = new TimeSpan(0, 0, 5);

            var source = new TokenDataModel()
            {
                AccountId = "fake_user_id",
                Name = "unittest",
                Mob = false,
                Kind = (byte)TokenKindEnum.AccessToken,
                LV = (byte)TokenLevelEnum.EndUser,
                Iss = m_AuthClient.GetType().Name,
                Exp = now + expiredInSpan,
                Upt = now,
                Iat = now
            };

            var encrypted = await m_AuthClient.Encode(source);
            Assert.NotNull(encrypted);

            {
                var isAdmin = await m_AuthClient.IsAdmin(encrypted);
                Assert.False(isAdmin);

                var isValid = await m_AuthClient.IsGeneratedFromAuthServer(encrypted);
                Assert.True(isValid);

                var isExpired = await m_AuthClient.IsExpired(encrypted);
                Assert.False(isExpired);

                var decoded = await m_AuthClient.Decode(encrypted);
                Assert.False(string.IsNullOrWhiteSpace(decoded));

                var tokenModel = await m_AuthClient.Deserialize<TokenDataModel>(encrypted);
                Assert.NotNull(tokenModel);
                Assert.Equal(source.AccountId, tokenModel.AccountId);
                Assert.Equal(source.Name, tokenModel.Name);

                var verified = await m_AuthClient.VerifyToken<TokenDataModel>(encrypted,
                    TokenKindEnum.AccessToken
                );
                Assert.True(verified.IsSuccess);
                Assert.NotNull(verified.Data);
                Assert.Equal(source.AccountId, verified.Data.AccountId);
                Assert.Equal(source.Name, verified.Data.Name);
            }

            var renewed = await m_AuthClient.RenewToken(encrypted);
            Assert.False(string.IsNullOrWhiteSpace(renewed));
            Assert.NotEqual(encrypted, renewed);

            // decode agagin
            {
                var isAdmin = await m_AuthClient.IsAdmin(renewed);
                Assert.False(isAdmin);

                var isValid = await m_AuthClient.IsGeneratedFromAuthServer(renewed);
                Assert.True(isValid);

                var isExpired = await m_AuthClient.IsExpired(renewed);
                Assert.False(isExpired);

                var decoded = await m_AuthClient.Decode(renewed);
                Assert.False(string.IsNullOrWhiteSpace(decoded));

                var tokenModel = await m_AuthClient.Deserialize<TokenDataModel>(renewed);
                Assert.NotNull(tokenModel);
                Assert.Equal(source.AccountId, tokenModel.AccountId);
                Assert.Equal(source.Name, tokenModel.Name);

                var verified = await m_AuthClient.VerifyToken<TokenDataModel>(renewed,
                    TokenKindEnum.AccessToken
                );
                Assert.True(verified.IsSuccess);
                Assert.NotNull(verified.Data);
                Assert.Equal(source.AccountId, verified.Data.AccountId);
                Assert.Equal(source.Name, verified.Data.Name);
            }

            // expired
            {
                Thread.Sleep(expiredInSpan.Seconds * 1000);

                var isValid = await m_AuthClient.IsGeneratedFromAuthServer(renewed);
                Assert.True(isValid);

                {
                    var isExpired = await m_AuthClient.IsExpired(source);
                    Assert.True(isExpired);
                }

                {
                    var isExpired = await m_AuthClient.IsExpired(renewed);
                    Assert.False(isExpired);
                }
            }

            // Revoke refreshToken by change version
            {
                await m_AuthClient.IncreseMinimumRefershTokenVersion();
                source.Kind = (byte)TokenKindEnum.RefreshToken;
                encrypted = await m_AuthClient.Encode(source);
                var isExpired = await m_AuthClient.IsExpired(encrypted);
                Assert.True(isExpired);
            }
        }
    }
}
