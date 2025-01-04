using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.ServiceNode.ServiceStack.Attributes;

namespace Nwpie.Foundation.S3Proxy.Endpoint.Attributes
{
    public class CustomTokenFilterAsyncAttribute : TokenFilterAsyncAttribute
    {
        public override async Task<bool?> IsValidAccount(ITokenDataModel tokenModel)
        {
            await Task.CompletedTask;
            return true == tokenModel?.GetAccountId().HasValue();
        }

        public override ITokenService GetTokenService(HttpRequest request)
        {
            var token = TokenUtils.GetTokenFromHeaderOrQuery(request);
            if (token.IsAttachBearer())
            {
                return ComponentMgr.Instance.GetDefaultJwtTokenService();
            }

            if (token.HasValue())
            {
                return ComponentMgr.Instance.GetDefaultAesTokenService();
            }

            return ComponentMgr.Instance.GetDefaultTokenService();
        }
    }
}
