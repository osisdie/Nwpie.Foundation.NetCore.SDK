using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.Storage.Contract.Assets.Exists;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Exists.Interfaces
{
    public interface IDamExists_DomainService : IDomainService
    {
        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Task<DamExists_Response> Execute(DamExists_Request param);
    }
}
