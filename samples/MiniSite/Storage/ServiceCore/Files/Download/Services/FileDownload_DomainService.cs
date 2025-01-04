using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.MiniSite.Storage.Common.Services;
using Nwpie.MiniSite.Storage.Contract.File.Download;
using Nwpie.MiniSite.Storage.ServiceCore.Files.Download.Interfaces;

namespace Nwpie.MiniSite.Storage.ServiceCore.Files.Download.Services
{
    public class FileDownload_DomainService :
        DomainService,
        IFileDownload_DomainService
    {
        public async Task<FileDownload_StreamResponse> Execute(FileDownload_Request param)
        {
            Validate(param);

            if (true == param.Bucket?.Contains("dummy", StringComparison.OrdinalIgnoreCase))
            {
                return await ExecuteFake(param);
            }

            var s3Result = await GetStorage().GetFileAsync(param.Bucket, param.Key);
            if (false == s3Result?.IsSuccess)
            {
                SubCode = s3Result.Code > 0 ? ((StatusCodeEnum)s3Result.Code).ToString() : "";
                SubMsg = s3Result.ErrMsg ?? s3Result.Msg;
                throw new Exception(SubMsg);
            }

            if (s3Result.Any())
            {
                var extension = Path.GetExtension(Path.GetFileName(param.Key));
                var contentType = WebUtils.GetHttpResponseContentType(extension.TrimStart('.'));

                var stream = new MemoryStream(s3Result.Data);
                return true == param.Inline
                    ? new FileDownload_StreamResponse(stream, contentType)
                    : new FileDownload_StreamResponse(stream, contentType, param.Key);
            }

            return null;
        }

        public async Task<FileDownload_StreamResponse> ExecuteFake(FileDownload_Request param)
        {
            const string randomPhotoUrl = "https://dummyimage.com/600x400/000/fff";
            Stream stream = null;
            using (var webClient = new WebClient())
            {
                stream = await webClient.OpenReadTaskAsync(randomPhotoUrl);
            }

            var extension = Path.GetExtension(Path.GetFileName(param.Key));
            var contentType = WebUtils.GetHttpResponseContentType(extension.TrimStart('.'));
            using (stream)
            {
                return true == param.Inline
                    ? new FileDownload_StreamResponse(stream, contentType)
                    : new FileDownload_StreamResponse(stream, contentType, param.Key);
            }
        }

        public bool Validate(FileDownload_Request param)
        {
            if (true == param?.Bucket?.Contains("/") || true == param?.Bucket?.Contains("\\"))
            {
                throw new InvalidOperationException($"Invalid bucket name. Bucket: {param?.Bucket}.");
            }

            if (true == param?.Key?.StartsWith("/") || true == param?.Key?.StartsWith("\\"))
            {
                throw new InvalidOperationException($"Invalid fileKey name. FileKey: {param?.Key}.");
            }

            if (null == GetStorage())
            {
                throw new InvalidOperationException("Storage service is not ready. ");
            }

            if (null == GetApplicationAccountId())
            {
                throw new InvalidOperationException("Storage service account is not ready. ");
            }

            return base.ValidateAndThrow(param);
        }
    }
}
