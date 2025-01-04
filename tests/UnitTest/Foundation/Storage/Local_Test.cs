using System;
using System.IO;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Common.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Storage
{
    public class Local_Test : TestBase
    {
        public Local_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Upload_Test()
        {
            IStorage client = ComponentMgr.Instance.TryResolve<ILocalStorageClient>();
            Assert.NotNull(client);

            //IStorage client2 = ComponentMgr.Instance.TryResolve<IStorage>();
            //Assert.NotNull(client2);
            //Assert.Same(client, client2);

            var bytes = Convert.FromBase64String(ConfigConst.FakeImageData);
            var path = $"results/test/{Utility.GetCallerFullName(2)}-{DateTime.Now:yyyyMMddHHmmss}.png";
            using (var ms = new MemoryStream(bytes))
            {
                var taskUpload = await client.UploadAsync(m_BucketName, path, ms);
                Assert.True(taskUpload.IsSuccess);
                Assert.True(taskUpload.Data);
            }

            var taskdownload = await client.GetFileAsync(m_BucketName, path);
            Assert.True(taskdownload.IsSuccess);
            Assert.NotEmpty(taskdownload.Data);

            var taskUrl = await client.GetPreSignedURLAsync(
                m_BucketName,
                path,
                DateTime.UtcNow.AddMinutes(5)
            );
            Assert.True(taskUrl.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(taskUrl.Data));
        }

        public override Task<bool> IsReady()
        {
            var fakeS3option = ComponentMgr.Instance.TryResolve<IConfigOptions<LocalStorage_Option>>();
            Assert.NotNull(fakeS3option);
            Assert.NotNull(fakeS3option.Value);
            Assert.NotNull(fakeS3option.Value.BucketName);
            m_BucketName = fakeS3option.Value.BucketName;

            return base.IsReady();
        }

        protected string m_BucketName;
    }
}
