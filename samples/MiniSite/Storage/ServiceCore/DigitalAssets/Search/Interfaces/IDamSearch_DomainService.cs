using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.Storage.Contract.Assets.Search;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Search.Interfaces
{
    public interface IDamSearch_DomainService : IDomainService
    {
        /// <summary>
        /// List resources
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Task<DamSearch_Response> Execute(DamSearch_Request param);
    }
}
