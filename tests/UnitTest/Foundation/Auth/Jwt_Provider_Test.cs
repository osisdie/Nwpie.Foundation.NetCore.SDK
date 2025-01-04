using System;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Auth
{
    public class Jwt_Provider_Test : AuthTestBase
    {
        public Jwt_Provider_Test(ITestOutputHelper output) : base(output) { }

        async Task NotValidToken_Test(string encrypted)
        {
            {
                var result = await m_AuthClient.VerifyToken<TokenDataModel>(encrypted);
                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Data);
            }
        }

        [Fact]
        public async Task Jwt_AdminToken_Test()
        {
            {
                var isAdmin = await m_AuthClient.IsAdmin(m_AuthOption.AdminToken);
                Assert.True(isAdmin);
                var isAdminWithBearer = await m_AuthClient.IsAdmin(m_AuthOption.AdminToken.AttachBearer());
                Assert.True(isAdminWithBearer);

                var isValid = await m_AuthClient.IsGeneratedFromAuthServer(m_AuthOption.AdminToken);
                Assert.True(isValid);
                var isValidWithBearer = await m_AuthClient.IsGeneratedFromAuthServer(m_AuthOption.AdminToken.AttachBearer());
                Assert.True(isValidWithBearer);

                var isExpired = await m_AuthClient.IsExpired(m_AuthOption.AdminToken);
                Assert.False(isExpired);
                var isExpiredWithBearer = await m_AuthClient.IsExpired(m_AuthOption.AdminToken.AttachBearer());
                Assert.False(isExpiredWithBearer);
            }

            var decoded = await m_AuthClient.Decode(m_AuthOption.AdminToken);
            Assert.False(string.IsNullOrWhiteSpace(decoded));

            var decodedWithBearer = await m_AuthClient.Decode(m_AuthOption.AdminToken.AttachBearer());
            Assert.False(string.IsNullOrWhiteSpace(decodedWithBearer));

            var tokenModel = await m_AuthClient.Deserialize<TokenDataModel>(m_AuthOption.AdminToken);
            Assert.NotNull(tokenModel);
            Assert.NotNull(tokenModel.AccountId);
            Assert.NotNull(tokenModel.Name);
            Assert.Equal((byte)TokenLevelEnum.ApplicationUser, tokenModel.LV);

            var tokenModelWithBearer = await m_AuthClient.Deserialize<TokenDataModel>(m_AuthOption.AdminToken.AttachBearer());
            Assert.NotNull(tokenModelWithBearer);
            Assert.NotNull(tokenModelWithBearer.AccountId);
            Assert.NotNull(tokenModelWithBearer.Name);
            Assert.Equal((byte)TokenLevelEnum.ApplicationUser, tokenModel.LV);

            var verified = await m_AuthClient.VerifyToken<TokenDataModel>(m_AuthOption.AdminToken,
                TokenKindEnum.AccessToken
            );
            Assert.True(verified.IsSuccess);
            Assert.NotNull(verified.Data);
            Assert.NotNull(verified.Data.AccountId);
            Assert.NotNull(verified.Data.Name);
            Assert.Equal((byte)TokenLevelEnum.ApplicationUser, tokenModel.LV);

            var verifiedWithBearer = await m_AuthClient.VerifyToken<TokenDataModel>(
                m_AuthOption.AdminToken.AttachBearer(),
                TokenKindEnum.AccessToken
            );
            Assert.True(verifiedWithBearer.IsSuccess);
            Assert.NotNull(verifiedWithBearer.Data);
            Assert.NotNull(verifiedWithBearer.Data.AccountId);
            Assert.NotNull(verifiedWithBearer.Data.Name);
            Assert.Equal((byte)TokenLevelEnum.ApplicationUser, tokenModel.LV);

            var renewed = await m_AuthClient.RenewToken(m_AuthOption.AdminToken);
            Assert.False(string.IsNullOrWhiteSpace(renewed));

            var renewedWithBearer = await m_AuthClient.RenewToken(m_AuthOption.AdminToken.AttachBearer());
            Assert.False(string.IsNullOrWhiteSpace(renewedWithBearer));

            // Revoke refreshToken by change version (except Admin)
            {
                {
                    var isExpired = await m_AuthClient.IsExpired(renewed);
                    Assert.False(isExpired);
                }

                {
                    var isExpired = await m_AuthClient.IsExpired(renewedWithBearer);
                    Assert.False(isExpired);
                }

                await m_AuthClient.IncreseMinimumRefershTokenVersion();

                {
                    var isExpired = await m_AuthClient.IsExpired(renewed);
                    Assert.False(isExpired);
                }

                {
                    var isExpired = await m_AuthClient.IsExpired(renewedWithBearer);
                    Assert.False(isExpired);
                }
            }
        }

        [Fact]
        public async Task Jwt_UserAccessToken_Test()
        {
            var expiredInSpan = new TimeSpan(0, 0, 5);
            var acctId = "fake_user_id";
            var acctName = "unittest";
            var tokenized = await GenerateTokenExpiresAfter(expiredInSpan, acctId, acctName);
            var source = tokenized.source;
            var encrypted = tokenized.encrypted;
            Assert.NotNull(encrypted);

            encrypted = encrypted.AttachBearer();

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

            renewed = renewed.AttachBearer();
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
                await NotValidToken_Test(encrypted);

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
