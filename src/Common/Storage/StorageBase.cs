using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Storage.Models;
using Nwpie.Foundation.Common.Cache;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Nwpie.Foundation.Common.Storage
{
    public abstract class StorageBase : CObject, IStorage
    {
        public StorageBase(ICache cache) : base()
        {
            m_CacheClient = cache ?? new DefaultMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                TrackStatistics = true
            }));
        }

        public virtual async Task<IServiceResponse<bool>> IsBuckekExistAsync(string bucketName)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<bool>> CreateBucketAsync(string bucketName)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, string fullFilePathInLocal)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, Stream streamUpload)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<bool>> UploadAsync(string bucketName, string fullFilePathInBucket, byte[] aryBytesUpload)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<byte[]>> GetFileAsync(string bucketName, string fullFilePathInBucket, string versionId = null)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<bool>> IsFileExistAsync(string bucketName, string fullFilePathInBucket, string versionId = null)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<string>> GetFileUrlAsync(string bucketName, string fullFilePathInBucket, int ttlDays, string versionId = null)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<string>> GetPreSignedURLAsync(string bucketName, string fullFilePathInBucket, DateTime dtExpires, string versionId = null)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<List<IStorageVersionDetail>>> ListVersionsAsync(string bucketName, string fullFilePathInBucket, int maxVersions = ConfigConst.MaxVersions)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async Task<IServiceResponse<string>> CopyVersionToLatestAsync(string bucketName, string fullFilePathInBucket, string versionId, IDictionary<string, string> metadata = null)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        protected ICache m_CacheClient;
    }
}
