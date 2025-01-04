using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.Storage.Contract.Assets.Upload;
using Microsoft.AspNetCore.Http;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Upload.Interfaces
{
    public interface IDamUpload_DomainService : IDomainService
    {
        /// <summary>
        /// private upload a file
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Task<DamUpload_Response> Execute(DamUpload_Request param);
        Task<DamUpload_Response> Execute(DamUpload_Request param, IFormFile file);
    }
}
