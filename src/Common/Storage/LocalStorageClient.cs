using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Common.Storage
{
    public class LocalStorageClient : StorageBase, ILocalStorageClient
    {
        public LocalStorageClient(IConfigOptions<LocalStorage_Option> option, ICache cache)
            : base(cache)
        {
            m_Option = option ?? throw new ArgumentNullException(nameof(IConfigOptions<LocalStorage_Option>));
            if (null == m_Option.Value)
            {
                throw new ArgumentNullException(nameof(option.Value));
            }

            m_Option.Value.BasePath = m_Option.Value.BasePath.AssignIfNotSet(AppDomain.CurrentDomain.BaseDirectory);
        }

        public override async Task<IServiceResponse<bool>> IsBuckekExistAsync(string bucketName)
        {
            var result = new ServiceResponse<bool>();

            try
            {
                var isExist = Directory.Exists(Path.Combine(
                    m_Option.Value.BasePath,
                    bucketName
                ));

                if (true == isExist)
                {
                    result.Success(StatusCodeEnum.Success, $"bucket(={bucketName}) exists. ")
                        .Content(true);
                }
                else
                {
                    result.Success(StatusCodeEnum.Success, $"bucket(={bucketName}) NOT exists. ")
                        .Content(true);
                }
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, ex={ex} ");
                throw;
            }

            await Task.CompletedTask;
            return result;
        }

        public override async Task<IServiceResponse<bool>> CreateBucketAsync(string bucketName)
        {
            var result = new ServiceResponse<bool>();

            try
            {
                var created = Directory.CreateDirectory(Path.Combine(
                    BasePath,
                    bucketName
                ));

                result.Success().Content(created?.Exists ?? false);
            }

            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, ex={ex} ");
                throw;
            }

            await Task.CompletedTask;
            return result;
        }

        public override async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, string fullFilePathInLocal)
        {
            var result = new ServiceResponse<bool>();
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"Try to upload file to not exist bucket, bucket={bucketName}, file={fullFilePathInBucket}, local={fullFilePathInLocal} ");
                return result;
            }

            if (false == File.Exists(fullFilePathInLocal))
            {
                result.Error(StatusCodeEnum.Exception, $"Try to upload non-exist file, bucket={bucketName}, file={fullFilePathInBucket}, local={fullFilePathInLocal} ");
                return result;
            }

            using (var fsFileData = new FileStream(fullFilePathInLocal, FileMode.Open, FileAccess.Read))
            {
                return await UploadAsync(bucketName, fullFilePathInBucket, fsFileData);
            }
        }

        public override async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, Stream streamUpload)
        {
            var result = new ServiceResponse<bool>();
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"Try to upload file to not existing bucket, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            try
            {
                var path = Path.Combine(BasePath, bucketName, fullFilePathInBucket);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var stream = File.OpenWrite(path))
                {
                    await streamUpload.CopyToAsync(stream);
                }

                result.Success().Content(true);
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                throw;
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
            if (true != buckekStatus?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"Try to get file from not existing bucket, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            var chkResult = await IsFileExistAsync(bucketName, fullFilePathInBucket);
            if (true != chkResult?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"File not found, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            try
            {
                var bytes = File.ReadAllBytes(Path.Combine(
                    BasePath,
                    bucketName,
                    fullFilePathInBucket
                ));

                if (bytes?.Count() > 0)
                {
                    result.Success().Content(bytes);
                }
                else
                {
                    result.Success(StatusCodeEnum.EmptyData);
                }
            }

            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket} ");
                Logger.LogError($"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                throw;
            }

            return result;
        }

        /// <summary>
        /// 其實是回傳 GetPreSignedURL
        /// 預設+1d過期, url會被server存下來
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
                _ = m_CacheClient.SetAsync(cacheKey, result.Data, (int)(dtExpires - DateTime.UtcNow).TotalSeconds);
            }

            return result;
        }

        /// <summary>
        /// 回傳 getPreSignedURL
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="dtExpires">Utc過期時間</param>
        /// <returns></returns>
        public override async Task<IServiceResponse<string>> GetPreSignedURLAsync(string bucketName, string fullFilePathInBucket, DateTime dtExpires, string versionId = null)
        {
            var result = new ServiceResponse<string>();
            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"Try to get presigned url from not exist bucket, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            var chkResult = await IsFileExistAsync(bucketName, fullFilePathInBucket);
            if (true != chkResult?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"File not found, bucket={bucketName}, file={fullFilePathInBucket} ");
                return result;
            }

            try
            {
                result.Success().Content(Path.Combine(
                    BasePath,
                    bucketName,
                    fullFilePathInBucket
                ));
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, $"Exception, bucket={bucketName}, file={fullFilePathInBucket}, ex={ex} ");
                throw;
            }

            return result;
        }

        public override async Task<IServiceResponse<bool>> IsFileExistAsync(string bucketName, string fullFilePathInBucket, string versionId = null)
        {
            var result = new ServiceResponse<bool>();
            if (false == bucketName.HasValue() ||
                false == fullFilePathInBucket.HasValue())
            {
                throw new ArgumentNullException($"Missing {nameof(bucketName)} or {nameof(fullFilePathInBucket)}, bucket={bucketName}, file={fullFilePathInBucket} ");
            }

            var buckekStatus = await IsBuckekExistAsync(bucketName);
            if (true != buckekStatus?.Data)
            {
                result.Error(StatusCodeEnum.Exception, $"Failed to check file is existing in the bucket={bucketName} ");
                return result;
            }

            try
            {
                var isExist = File.Exists(Path.Combine(
                    m_Option.Value.BasePath,
                    bucketName,
                    fullFilePathInBucket
                ));

                result.Success().Content(isExist);
            }
            catch (Exception)
            {
                result.Success().Content(false);
            }

            return result;
        }

        public string BasePath
        {
            get => m_Option?.Value?.BasePath;
        }

        protected IConfigOptions<LocalStorage_Option> m_Option;
    }
}
