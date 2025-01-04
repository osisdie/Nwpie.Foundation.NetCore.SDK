using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Auth
{
    public class TokenProfileMgr_Test : AuthTestBase
    {
        public TokenProfileMgr_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task GetApplicationProfile_Test()
        {
            var profile = await TokenProfileMgr.Instance
                .GetProfileByAccountIdAsync<AccountProfileBase>(m_TestAppId);
            Assert.NotNull(profile?.AccountId);
        }

        [Fact(Skip = "Takes time")]
        public async Task PurgeProfile_Test()
        {
            Thread.Sleep(TokenProfileMgr.DefaultDelayStart);

            var profile = await TokenProfileMgr.Instance
                .GetProfileByAccountIdAsync<AccountProfileBase>(m_TestAppId);
            Assert.NotNull(profile?.AccountId);

            var exists = TokenProfileMgr.Instance.GetProfile<AccountProfileBase>(m_TestAppId);
            Assert.NotNull(exists);

            Thread.Sleep(TokenProfileMgr.DefaultFlushInterval);
            Thread.Sleep(TokenProfileMgr.DefaultPersistSeconds * 1000);
            Thread.Sleep(TokenProfileMgr.DefaultFlushInterval);

            var notExists = TokenProfileMgr.Instance.GetProfile<AccountProfileBase>(m_TestAppId);
            Assert.Null(notExists);
        }

        private readonly string m_TestAppId = "fake_app_id.debug";
    }
}
