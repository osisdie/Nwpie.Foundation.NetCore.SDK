using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Storage
{
    public class S3_Test : TestBase
    {
        public S3_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "make sure credential is ready")]
        public async Task NativeS3Client_Test()
        {
            var bucketName = "asset-api-dev";
            var option = new AwsS3_Option
            {
                AccessKey = "**",
                SecretKey = "**",
                Region = RegionEndpoint.USWest2.SystemName
            };

            var s3Config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(option.Region)
            };

            var client = new AmazonS3Client(option.AccessKey, option.SecretKey, s3Config);
            var path = $"test/upload.txt";

            {
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = path
                };

                using (var objResponse = await client.GetObjectAsync(getObjectRequest))
                {
                    using (var respStream = objResponse.ResponseStream)
                    {
                        var buffer = new byte[16 * 1024];
                        using (var stream = new MemoryStream())
                        {
                            int read;
                            while ((read = respStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                stream.Write(buffer, 0, read);
                            }

                            var words = Encoding.UTF8.GetString(buffer);
                            Assert.NotNull(words);
                        }
                    }
                }
            }

            string srcVersionId = null;
            {
                var request = new ListVersionsRequest()
                {
                    BucketName = bucketName,
                    Prefix = path,
                    MaxKeys = ConfigConst.MaxVersions
                };

                var response = await client.ListVersionsAsync(request);
                Assert.True(response?.Versions?.Count > 0);

                srcVersionId = (response.Versions.Skip(1).Take(1).FirstOrDefault()
                    ?? response.Versions.First()).VersionId;
            }

            {
                var request = new CopyObjectRequest()
                {
                    SourceBucket = bucketName,
                    SourceKey = path,
                    SourceVersionId = srcVersionId, // "_nzI3_yiLvJZbFyFwKjFcweFFKZIDWC4"
                    DestinationBucket = bucketName,
                    DestinationKey = path,
                };

                // useless
                // request.Metadata.Add(nameof(CopyObjectRequest.SourceVersionId), preVersionId);

                var response = await client.CopyObjectAsync(request);
                Assert.NotNull(response?.VersionId);
            }
        }

        [Fact(Skip = "Won't test remote s3 service")]
        public async Task CustomCredentialClient_Test()
        {
            var bucketName = "your bucket name";
            var factory = m_S3Factory.GetService(new ConfigOptions<AwsS3_Option>
            {
                Value = new AwsS3_Option
                {
                    AccessKey = "**",
                    SecretKey = "**",
                    Region = RegionEndpoint.USWest2.SystemName
                }
            });
            Assert.NotNull(factory);
            Assert.NotNull(factory.Data);
            var client = factory.Data;

            var bytes = Convert.FromBase64String(ConfigConst.FakeImageData);
            var path = $"test/{Utility.GetCallerFullName(2)}-{DateTime.Now:yyyyMMddHHmmss}.png";
            using (var stream = new MemoryStream(bytes))
            {
                var taskUpload = await client.UploadAsync(bucketName, path, stream);
                Assert.True(taskUpload.IsSuccess);
                Assert.True(taskUpload.Data);
            }

            var taskdownload = await client.GetFileAsync(bucketName, path);
            Assert.True(taskdownload.IsSuccess);
            Assert.NotEmpty(taskdownload.Data);

            {
                var taskUrl = await client.GetPreSignedURLAsync(bucketName, path,
                    DateTime.UtcNow.AddMinutes(5)
                );
                Assert.True(taskUrl.IsSuccess);
                Assert.False(string.IsNullOrWhiteSpace(taskUrl.Data));
                m_Output.WriteLine($"expired in 5 mins: {taskUrl.Data}");
            }

            {
                var taskUrl1d = await client.GetFileUrlAsync(bucketName, path, 1440);
                Assert.True(taskUrl1d.IsSuccess);
                Assert.False(string.IsNullOrWhiteSpace(taskUrl1d.Data));
                m_Output.WriteLine($"expired in 1 day: {taskUrl1d.Data}");

                var cachedTaskUrl1d = await client.GetFileUrlAsync(bucketName, path, 1440);
                Assert.True(cachedTaskUrl1d.IsSuccess);
                Assert.Equal(taskUrl1d.Data, cachedTaskUrl1d.Data);
            }
        }

        [Fact(Skip = "Won't test remote s3 service")]
        public async Task Upload_Test()
        {
            var bucketName = m_BucketName;
            IStorage client = ComponentMgr.Instance.TryResolve<IS3StorageClient>();
            Assert.NotNull(client);

            //IStorage client2 = ComponentMgr.Instance.TryResolve<IStorage>();
            //Assert.NotNull(client2);
            //Assert.Same(client, client2);

            var bytes = Convert.FromBase64String(ConfigConst.FakeImageData);
            var path = $"results/test/{Utility.GetCallerFullName(2)}-{DateTime.Now:yyyyMMddHHmmss}.png";
            using (var stream = new MemoryStream(bytes))
            {
                var taskUpload = await client.UploadAsync(bucketName, path, stream);
                Assert.True(taskUpload.IsSuccess);
                Assert.True(taskUpload.Data);
            }

            var taskdownload = await client.GetFileAsync(bucketName, path);
            Assert.True(taskdownload.IsSuccess);
            Assert.NotEmpty(taskdownload.Data);

            {
                var taskUrl = await client.GetPreSignedURLAsync(bucketName, path,
                    DateTime.UtcNow.AddMinutes(5)
                );
                Assert.True(taskUrl.IsSuccess);
                Assert.False(string.IsNullOrWhiteSpace(taskUrl.Data));
                m_Output.WriteLine($"expired in 5 mins: {taskUrl.Data}");

            }

            {
                var taskUrl1d = await client.GetFileUrlAsync(bucketName, path, 1440);
                Assert.True(taskUrl1d.IsSuccess);
                Assert.False(string.IsNullOrWhiteSpace(taskUrl1d.Data));
                m_Output.WriteLine($"expired in 1 day: {taskUrl1d.Data}");

                var cachedTaskUrl1d = await client.GetFileUrlAsync(bucketName, path, 1440);
                Assert.True(cachedTaskUrl1d.IsSuccess);
                Assert.Equal(taskUrl1d.Data, cachedTaskUrl1d.Data);
            }
        }

        [Fact(Skip = "Won't test remote s3 service")]
        public async Task UploadByS3Mgr_Test()
        {
            var bucketName = m_BucketName;
            var opt = ComponentMgr.Instance.TryResolve<IConfigOptions<AwsS3_Option>>();
            Assert.NotNull(opt);

            var factory = m_S3Factory.GetService(opt);
            Assert.NotNull(factory);
            Assert.NotNull(factory.Data);
            var client = factory.Data;

            var bytes = Convert.FromBase64String(ConfigConst.FakeImageData);
            var path = $"results/test/{Utility.GetCallerFullName(2)}-{DateTime.Now:yyyyMMddHHmmss}.png";
            using (var stream = new MemoryStream(bytes))
            {
                var taskUpload = await client.UploadAsync(bucketName, path, stream);
                Assert.True(taskUpload.IsSuccess);
                Assert.True(taskUpload.Data);
            }

            var taskdownload = await client.GetFileAsync(bucketName, path);
            Assert.True(taskdownload.IsSuccess);
            Assert.NotEmpty(taskdownload.Data);

            var taskUrl = await client.GetPreSignedURLAsync(
                bucketName,
                path,
                DateTime.UtcNow.AddMinutes(60)
            );
            Assert.True(taskUrl.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(taskUrl.Data));
        }

        [Fact(Skip = "Won't test remote s3 service")]
        public async Task HongKongPreSignedURL_Test()
        {
            var configValue = GetMapValueDefault(
                SysConfigKey.PrefixKey_AWS_S3_Credential_ConfigKey + "todo_db",
                EnvironmentEnum.Production.GetDisplayName()
            );
            Assert.NotNull(configValue);

            var opt = new ConfigOptions<AwsS3_Option>(
                Serializer.Deserialize<AwsS3_Option>(configValue)
            );
            Assert.NotNull(opt);
            Assert.NotNull(opt.Value);
            opt.Value.Region = Amazon.RegionEndpoint.APEast1.SystemName;

            var tryGetS3Client = m_S3Factory.GetService(opt);
            Assert.NotNull(tryGetS3Client);
            Assert.NotNull(tryGetS3Client.Data);
            var client = tryGetS3Client.Data;

            var taskUrl = await client.GetPreSignedURLAsync("ds1.hongkong.s3.test",
                "1562658900875.jpg",
                DateTime.UtcNow.AddDays(2)
            );
            Assert.True(taskUrl.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(taskUrl.Data));
        }

        [Fact(Skip = "Won't test remote s3 service")]
        public async Task Version_Test()
        {
            var bucketName = "asset-api-dev";
            var opt = ComponentMgr.Instance.TryResolve<IConfigOptions<AwsS3_Option>>();
            Assert.NotNull(opt);

            opt.Value.AccessKey = "**";
            opt.Value.SecretKey = "**";

            var factory = m_S3Factory.GetService(opt);
            Assert.NotNull(factory);
            Assert.NotNull(factory.Data);
            var client = factory.Data;

            var path = $"test/upload.txt";
            string srcVersionId = null;
            {
                var response = await client.ListVersionsAsync(bucketName, path);
                Assert.True(response?.IsSuccess);
                Assert.True(response.Data?.Count > 0);

                srcVersionId = (response.Data.Skip(1).Take(1).FirstOrDefault()
                    ?? response.Data.First()).VersionId;
            }

            {
                var response = await client.CopyVersionToLatestAsync(bucketName, path, srcVersionId);
                Assert.True(response?.IsSuccess);
                Assert.NotNull(response.Data);
                Assert.NotEqual(srcVersionId, response.Data);
            }
        }

        public override Task<bool> IsReady()
        {
            m_S3Factory = ComponentMgr.Instance.TryResolve<IAwsS3Factory>();
            Assert.NotNull(m_S3Factory);

            var s3option = ComponentMgr.Instance.TryResolve<IConfigOptions<AwsS3_Option>>();
            Assert.NotNull(s3option);
            Assert.NotNull(s3option.Value);
            Assert.NotNull(s3option.Value.Region);
            Assert.NotNull(s3option.Value.BucketName);

            var qcS3option = ServiceContext.Configuration
                .GetValue<AwsS3_Option>(
                    SysConfigKey.PrefixKey_AWS_S3_Credential_ConfigKey + "todo_db",
                    Serializer
                );
            Assert.NotNull(qcS3option);
            Assert.NotNull(qcS3option.Region);
            Assert.NotNull(qcS3option.BucketName);

            m_BucketName = qcS3option.BucketName;

            return base.IsReady();
        }

        protected IAwsS3Factory m_S3Factory;
        protected string m_BucketName;
    }
}
