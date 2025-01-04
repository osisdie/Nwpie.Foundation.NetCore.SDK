using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Storage.Models;

namespace Nwpie.Foundation.Common.Storage.Interfaces
{
    public interface IStorage : ICObject
    {
        Task<IServiceResponse<bool>> IsBuckekExistAsync(string bucketName);
        Task<IServiceResponse<bool>> CreateBucketAsync(string bucketName);
        Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, string fullFilePathInLocal);
        Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, Stream streamUpload);
        Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, byte[] aryBytesUpload);
        Task<IServiceResponse<byte[]>> GetFileAsync(string bucketName, string fullFilePathInBucket, string versionId = null);
        Task<IServiceResponse<bool>> IsFileExistAsync(string bucketName, string fullFilePathInBucket, string versionId = null);

        /// <summary>
        /// GetPreSignedURL with cached
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="ttlDays">UtcNow.AddDays</param>
        /// <returns></returns>
        Task<IServiceResponse<string>> GetFileUrlAsync(string bucketName, string fullFilePathInBucket, int ttlMinutes = ConfigConst.DefaultAssetUrlExpireMinutes, string versionId = null);

        /// <summary>
        /// Fetch PreSigned URL
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="dtExpires">Utc</param>
        /// <returns></returns>
        Task<IServiceResponse<string>> GetPreSignedURLAsync(string bucketName, string fullFilePathInBucket, DateTime dtExpires, string versionId = null);

        /// <summary>
        /// List file versions
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="maxVersions"> 1 ~ 1000, default 10</param>
        /// <returns>StorageVersionDetailModel Array</returns>
        Task<IServiceResponse<List<IStorageVersionDetail>>> ListVersionsAsync(string bucketName, string fullFilePathInBucket, int maxVersions = ConfigConst.MaxVersions);

        /// <summary>
        /// Restore specific version to be the latest version
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fullFilePathInBucket"></param>
        /// <param name="versionId"></param>
        /// <returns>Latest VersionId</returns>
        Task<IServiceResponse<string>> CopyVersionToLatestAsync(string bucketName, string fullFilePathInBucket, string versionId, IDictionary<string, string> metadata = null);
    }
}
