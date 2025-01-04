using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using ServiceStack;
using ServiceStack.Web;

namespace Nwpie.MiniSite.Storage.Contract.File.Download
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Storage:Private Download")]
    [Route("/Download", "GET")]
    public class FileDownload_Request :
        ContractRequestBase,
        IServiceReturn<FileDownload_StreamResponse>
    {
        [Required]
        [ApiMember(Description = "BucketName", IsRequired = true)]
        public string Bucket { get; set; }

        [Required]
        [ApiMember(Description = "Relative Path", IsRequired = true)]
        public string Key { get; set; }

        [ApiMember(Description = "Content-Disposition Header is inline mode ? Default: false")]
        public bool? Inline { get; set; }
    }

    public class FileDownload_StreamResponse : IHasOptions, IStreamWriterAsync
    {
        public FileDownload_StreamResponse(Stream responseStream, string contentType, string fileName = null)
        {
            m_ResponseStream = responseStream;
            Options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Content-Type", contentType },
                //{ "Content-Disposition", "attachment; filename=\"" + fileName + "\";" }
            };

            if (fileName.HasValue())
            {
                Options.Add("Content-Disposition", "attachment; filename=\"" + fileName + "\";");
            }
        }

        public async Task WriteToAsync(Stream responseStream, CancellationToken token = default)
        {
            if (null == m_ResponseStream)
            {
                return;
            }

            await m_ResponseStream.CopyToAsync(responseStream);
            await responseStream.FlushAsync();
        }

        public void Dispose()
        {
            m_ResponseStream?.Dispose();
        }

        private readonly Stream m_ResponseStream;
        public IDictionary<string, string> Options { get; private set; }
    }
    #endregion
}
