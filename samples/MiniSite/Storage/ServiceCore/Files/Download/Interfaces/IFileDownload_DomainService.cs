using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.Storage.Contract.File.Download;

namespace Nwpie.MiniSite.Storage.ServiceCore.Files.Download.Interfaces
{
    public interface IFileDownload_DomainService : IDomainService
    {
        /// <summary>
        /// Streamly download a file
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Task<FileDownload_StreamResponse> Execute(FileDownload_Request param);
    }
}
