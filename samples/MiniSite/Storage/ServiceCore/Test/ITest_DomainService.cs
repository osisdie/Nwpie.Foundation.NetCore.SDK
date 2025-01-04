using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.Storage.Contract.Test;

namespace Nwpie.MiniSite.Storage.ServiceCore.Test
{
    public interface ITest_DomainService : IDomainService
    {
        /// <summary>
        /// Test
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        Task<Test_Response> Execute(Test_Request param);
    }
}
