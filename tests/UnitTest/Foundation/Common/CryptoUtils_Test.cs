using System;
using System.IO;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Common
{
    public class CryptoUtils_Test : TestBase
    {
        public CryptoUtils_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Crypto_Test()
        {
            var rawData = "https://raw.githubusercontent.com/donma/TaiwanAddressCityAreaRoadChineseEnglishJSON/master/AllData.json";
            var encoded = CryptoUtils.EncodeToAES(rawData, m_AuthOption.AuthAESKey, m_AuthOption.AuthAESIV);
            Assert.NotNull(encoded);

            var decoded = CryptoUtils.DecodeFromAES(encoded, m_AuthOption.AuthAESKey, m_AuthOption.AuthAESIV);
            Assert.NotNull(encoded);

            Assert.Equal(rawData, decoded);
        }

        [Fact]
        public async Task Encrypt_Text_Test()
        {
            var rawData = "ds12019";
            var encoded = CryptoUtils.EncodeToAES(rawData, m_AuthOption.AuthAESKey, m_AuthOption.AuthAESIV);
            Assert.NotNull(encoded);

            var fullPath = $"/{ConfigConst.DefaultTempFolder}/encrypt_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.txt";
            await File.WriteAllTextAsync(fullPath, $"{rawData}{Environment.NewLine}{encoded}");
        }

        public override Task<bool> IsReady()
        {
            m_AuthOption = SysConfigKey
               .Default_Auth_ConfigKey
               .ConfigServerValue<Auth_Option>();
            Assert.NotNull(m_AuthOption?.AuthAESKey);
            Assert.NotNull(m_AuthOption?.AuthAESIV);

            return base.IsReady();
        }

        Auth_Option m_AuthOption;
    }
}
