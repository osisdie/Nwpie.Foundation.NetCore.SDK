using System.Net;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.Common.Attributes;
using Nwpie.MiniSite.Storage.Contract.Assets.Exists;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Exists.Interfaces;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Exists
{
    [CustomTokenFilterAsync]
    public class DamExists_Service : ApiServiceAnyInOutEntry<
    DamExists_Request,
    DamExists_Response,
    IDamExists_DomainService>
    {
        public override void OnResponseBodyWithoutDataProcessEnd(DamExists_Response response)
        {
            base.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
