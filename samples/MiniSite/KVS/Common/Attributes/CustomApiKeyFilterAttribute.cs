using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.ServiceNode.ServiceStack.Attributes;
using ServiceStack.Web;

namespace Nwpie.MiniSite.KVS.Common.Attributes
{
    public class CustomApiKeyFilterAttribute : ApiKeyFilterAsyncAttribute
    {
        public CustomApiKeyFilterAttribute()
        {
        }

        public override void OnError(IRequest req, IResponse res, StatusCodeEnum code)
        {
        }
    }
}
