using System.Net;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.Common.Attributes;
using Nwpie.MiniSite.Storage.Contract.Assets.Search;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Search.Interfaces;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Search
{
    [CustomTokenFilterAsync]
    public class DamSearch_Service : ApiServiceAnyInOutEntry<
    DamSearch_Request,
    DamSearch_Response,
    IDamSearch_DomainService>
    {
        public override void OnResponseBodyWithoutDataProcessEnd(DamSearch_Response response)
        {
            base.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
