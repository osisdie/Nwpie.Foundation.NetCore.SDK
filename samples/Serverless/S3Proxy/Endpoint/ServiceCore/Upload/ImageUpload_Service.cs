using System;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common.Config.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.S3Proxy.Contract;
using Nwpie.Foundation.S3Proxy.Contract.Upload;
using Nwpie.Foundation.S3Proxy.Endpoint.Attributes;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.Foundation.S3Proxy.Endpoint.ServiceCore.Upload
{
    [CustomTokenFilterAsync]
    public class ImageUpload_Service : Service
    {
        static ImageUpload_Service()
        {
            Logger = LogMgr.CreateLogger(typeof(ImageUpload_Service));
            m_S3Factory = ComponentMgr.Instance.TryResolve<IAwsS3Factory>();

            var opt = new ConfigOptions<AwsS3_Option>(
                S3ProxyServiceConfig.BucketConfigKeyForQC
                .ConfigServerValue<AwsS3_Option>()
            );
            BucketName = opt?.Value?.BucketName;
            if (string.IsNullOrEmpty(BucketName))
            {
                Logger.LogCritical("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket. ");
                throw new Exception("Missing configuration for S3 bucket. The AppS3Bucket configuration must be set to a S3 bucket. ");
            }

            m_S3Client = m_S3Factory.GetService(opt)?.Data;
            Logger.LogInformation($"Configured to use bucket {BucketName}");
        }

        public async Task<ImageUpload_Response> Any(ImageUpload_Request request)
        {
            Validate(request);

            var file = Request.Files.First();
            var response = new ImageUpload_Response();

            using (var stream = file.InputStream)
            {
                var uploaded = await m_S3Client.UploadAsync(BucketName, request.FileKey, stream);
                if (true != uploaded?.IsSuccess)
                {
                    throw new Exception($"file(={file.FileName}) failed to upload. msg={uploaded?.ErrMsg ?? uploaded?.Msg} ");
                }

                response.Success(StatusCodeEnum.Success,
                    $"Uploaded object {request.FileKey} to bucket {BucketName}. ");
                Logger.LogInformation($"Uploaded object {request.FileKey} to bucket {BucketName}. ");
            }

            return response;
        }

        public bool Validate(ImageUpload_Request param)
        {
            try
            {
                // Check a file has been attached
                if (1 != Request?.Files?.Count())
                {
                    throw new ArgumentException("Require only 1 file stream");
                }

                ValidateUtils.ValidateAndThrow(param);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                throw;
            }
        }

        private static readonly IAwsS3Factory m_S3Factory;

        private static string BucketName { get; set; }
        private static IStorage m_S3Client { get; set; }
        private static ILogger Logger { get; set; }
    }
}
