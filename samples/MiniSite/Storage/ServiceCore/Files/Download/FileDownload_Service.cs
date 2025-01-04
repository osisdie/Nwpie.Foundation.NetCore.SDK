using System.Net;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.Common.Attributes;
using Nwpie.MiniSite.Storage.Contract.File.Download;
using Nwpie.MiniSite.Storage.ServiceCore.Files.Download.Interfaces;

namespace Nwpie.MiniSite.Storage.ServiceCore.Files.Download
{
    [CustomTokenFilterAsync]
    public class FileDownload_Service : ApiServiceAnyInOutEntry<
    FileDownload_Request,
    FileDownload_StreamResponse,
    IFileDownload_DomainService>
    {
        public override void OnResponseBodyWithoutDataProcessEnd(FileDownload_StreamResponse response)
        {
            base.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
