using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Storage.Models;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Storage;
using Nwpie.Foundation.Storage.S3.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Storage.S3
{
    public class S3StorageClient : StorageBase, IS3StorageClient
    {
        public S3StorageClient(IConfigOptions<AwsS3_Option> option, ICache cache)
            : base(cache)
        {
            m_Option = option ?? throw new ArgumentNullException(nameof(AwsS3_Option));

            if (null == m_Option.Value?.Region)
            {
                throw new ArgumentNullException(nameof(AwsS3_Option.Region));
            }

            AwsRegionEndpoint = RegionEndpoint.GetBySystemName(m_Option.Value.Region);
            var s3Config = new AmazonS3Config()
            {
                RegionEndpoint = AwsRegionEndpoint
            };

            if (m_Option.Value.AccessKey.HasValue() &&
                m_Option.Value.SecretKey.HasValue())
            {
                m_S3Client = new AmazonS3Client(m_Option.Value.AccessKey, m_Option.Value.SecretKey, s3Config);
            }
            else
            {
                m_S3Client = new AmazonS3Client(s3Config); // use default credentials
            }
        }

        public override async Task<IServiceResponse<bool>> IsBuckekExistAsync(string bucketName)
        {
            var result = new ServiceResponse<bool>();

            try
            {
                var isExist = await AmazonS3Util.DoesS3BucketExistV2Async(m_S3Client, bucketName);
                if (isExist)
                {
                    result.Success(StatusCodeEnum.Success, $"bucket(={bucketName}) exists. ")
                        .Content(true);
                }
                else
                {
                    result.Success(StatusCodeEnum.Success, $"bucket(={bucketName}) NOT exists. ")
                        .Content(false);
                }
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, ex={ex} ");
            }

            return result;
        }

        public override async Task<IServiceResponse<bool>> CreateBucketAsync(string bucketName)
        {
            var result = new ServiceResponse<bool>();

            try
            {
                var chkExist = await AmazonS3Util.DoesS3BucketExistV2Async(m_S3Client, bucketName);
                if (false == chkExist)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    await m_S3Client.PutBucketAsync(putBucketRequest);

                    result.Success(StatusCodeEnum.Success, $"{nameof(CreateBucketAsync)} Success, bucket={bucketName} ")
                        .Content(true);
                }
                else
                {
                    result.Success(StatusCodeEnum.Success, $"{nameof(CreateBucketAsync)} failed, bucket={bucketName} arleady exists. ")
                        .Content(true);
                }
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, ex={ex} ");
            }

            return result;
        }

        public override async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, string fullFilePathInLocal)
        {
            var result = new ServiceResponse<bool>();
            if (false == File.Exists(fullFilePathInLocal))
            {
                result.Error(StatusCodeEnum.Exception, $"Cannot find local file, bucket={bucketName}, file={fullFilePathInBucket}, local={fullFilePathInLocal} ");
                return result;
            }

            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"Try to upload file to not exist bucket, bucket={bucketName}, file={fullFilePathInBucket}, local={fullFilePathInLocal} ");
                return result;
            }

            using (var stream = new FileStream(fullFilePathInLocal, FileMode.Open, System.IO.FileAccess.Read))
            {
                return await UploadAsync(bucketName, fullFilePathInBucket, stream);
            }
        }

        public override async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, Stream streamUpload)
        {
            var result = new ServiceResponse<bool>();
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"Try to upload file to not exist bucket, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            try
            {
                using (var transferUtility = new TransferUtility(m_S3Client))
                {
                    await transferUtility.UploadAsync(streamUpload, bucketName, fullFilePathInBucket);
                }

                result.Success(StatusCodeEnum.Success, $"{nameof(UploadAsync)} Success, bucket={bucketName}, file={fullFilePathInBucket} ")
                    .Content(true);
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }

            return result;
        }

        public override async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, byte[] aryBytesUpload)
        {
            using (Stream streamUpload = new MemoryStream(aryBytesUpload))
            {
                return await UploadAsync(bucketName, fullFilePathInBucket, streamUpload);
            }
        }

        public override async Task<IServiceResponse<byte[]>> GetFileAsync(string bucketName, string fullFilePathInBucket, string versionId = null)
        {
            var result = new ServiceResponse<byte[]>();
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"Try to get file from not exist bucket, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            var chkResult = await IsFileExistAsync(bucketName, fullFilePathInBucket);
            if (true != chkResult.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"File not found, region={AwsRegionEndpoint.DisplayName}, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            try
            {
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fullFilePathInBucket
                };

                using (var objResponse = await m_S3Client.GetObjectAsync(getObjectRequest))
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

                            result.Success(StatusCodeEnum.Success, $"Success, bucket={bucketName}, file={fullFilePathInBucket} ")
                                .Content(buffer);
                        }
                    }
                }
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }

            return result;
        }

        /// <summary>
        /// GetPreSignedURL with cached
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="ttlMinutes">UtcNow.AddMinutes</param>
        /// <returns></returns>
        public override async Task<IServiceResponse<string>> GetFileUrlAsync(string bucketName, string fullFilePathInBucket, int ttlMinutes = ConfigConst.DefaultAssetUrlExpireMinutes, string versionId = null)
        {
            IServiceResponse<string> result = new ServiceResponse<string>();
            var cacheKey = $"{ServiceContext.SdkEnv}.{nameof(GetFileUrlAsync)}.{bucketName}.{fullFilePathInBucket}".ToLower();
            if (null != m_CacheClient)
            {
                var cached = await m_CacheClient.GetAsync<string>(cacheKey).ConfigureAwait(false);
                if (cached.Any())
                {
                    result.Success().Content(cached.Data);
                    return result;
                }
            }

            var dtExpires = DateTime.UtcNow.AddMinutes(ttlMinutes);
            result = await GetPreSignedURLAsync(bucketName, fullFilePathInBucket, dtExpires);
            if (result.Any() && null != m_CacheClient)
            {
                _ = await m_CacheClient.SetAsync(cacheKey, result.Data, (int)(dtExpires - DateTime.UtcNow).TotalSeconds);
            }

            return result;
        }

        /// <summary>
        /// Fetch PreSigned URL
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="dtExpires">Utc</param>
        /// <returns></returns>
        public override async Task<IServiceResponse<string>> GetPreSignedURLAsync(string bucketName, string fullFilePathInBucket, DateTime dtExpires, string versionId = null)
        {
            var result = new ServiceResponse<string>();
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"Try to get presigned url from not existing bucket, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            var chkResult = await IsFileExistAsync(bucketName, fullFilePathInBucket);
            if (true != chkResult.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"File not found, region={AwsRegionEndpoint.DisplayName}, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = fullFilePathInBucket,
                Expires = dtExpires,
            };

            try
            {
                var url = m_S3Client.GetPreSignedURL(getPreSignedUrlRequest);
                if (url.HasValue())
                {
                    result.Success(StatusCodeEnum.Success, $"Success, bucket={bucketName}, file={fullFilePathInBucket} ")
                        .Content(url);
                }
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }

            return result;
        }

        public override async Task<IServiceResponse<bool>> IsFileExistAsync(string bucketName, string fullFilePathInBucket, string versionId = null)
        {
            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fullFilePathInBucket))
            {
                throw new ArgumentNullException($"Missing {nameof(bucketName)} or {nameof(fullFilePathInBucket)}, bucket={bucketName}, file={fullFilePathInBucket} ");
            }

            var result = new ServiceResponse<bool>(true);
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus.Any())
            {
                result.Error(StatusCodeEnum.Exception, $"Failed to check file is existing in the bucket={bucketName} ");
                return result;
            }

            try
            {
                var response = await m_S3Client.GetObjectMetadataAsync(bucketName, fullFilePathInBucket);

                result.Success().Content(true);
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    result.Success().Content(false);
                }
                else
                {
                    result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                    Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                }
            }
            catch (Exception)
            {
                result.Success().Content(false);
            }

            return result;
        }

        /// <summary>
        /// List file versions
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="maxVersions"> 1 ~ 1000, default 10</param>
        /// <returns>StorageVersionDetailModel Array</returns>
        public override async Task<IServiceResponse<List<IStorageVersionDetail>>> ListVersionsAsync(string bucketName, string fullFilePathInBucket, int maxVersions = ConfigConst.MaxVersions)
        {
            var result = new ServiceResponse<List<IStorageVersionDetail>>();
            var request = new ListVersionsRequest()
            {
                BucketName = bucketName,
                Prefix = fullFilePathInBucket,
                MaxKeys = maxVersions.AssignInRange(1, ConfigConst.MaxPageSize)
            };

            try
            {
                var response = await m_S3Client.ListVersionsAsync(request);
                if (response?.Versions?.Count > 0)
                {
                    result.Success(StatusCodeEnum.Success, $"Success, bucket={bucketName}, file={fullFilePathInBucket}, versions={response.Versions.Count} ")
                        .Content(response.Versions.Select(o => new StorageVersionDetailModel
                        {
                            VersionId = o.VersionId,
                            Size = o.Size,
                            IsLatest = o.IsLatest,
                            LastModified = o.LastModified
                        } as IStorageVersionDetail).ToList());
                }
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }

            return result;
        }

        /// <summary>
        /// Restore specific version to be the latest version
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="versionId"></param>
        /// <returns>Latest VersionId</returns>
        public override async Task<IServiceResponse<string>> CopyVersionToLatestAsync(string bucketName, string fullFilePathInBucket, string versionId, IDictionary<string, string> metadata = null)
        {
            var result = new ServiceResponse<string>();
            var request = new CopyObjectRequest()
            {
                SourceBucket = bucketName,
                SourceKey = fullFilePathInBucket,
                SourceVersionId = versionId,
                DestinationBucket = bucketName,
                DestinationKey = fullFilePathInBucket,
            };

            try
            {
                var response = await m_S3Client.CopyObjectAsync(request);
                if (true == response?.VersionId.HasValue())
                {
                    result.Success(StatusCodeEnum.Success, $"Success, bucket={bucketName}, file={fullFilePathInBucket}, from version={response.SourceVersionId}, to version={response.VersionId} ")
                        .Content(response.VersionId);
                }
            }
            catch (AmazonS3Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"{nameof(AmazonS3Exception)}, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
            }

            return result;
        }

        public Amazon.RegionEndpoint AwsRegionEndpoint { get; private set; }

        protected Amazon.S3.AmazonS3Client m_S3Client;
        protected IConfigOptions<AwsS3_Option> m_Option;
    }
}
