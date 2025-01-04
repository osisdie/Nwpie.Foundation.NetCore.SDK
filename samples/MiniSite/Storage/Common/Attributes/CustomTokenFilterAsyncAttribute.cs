using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.ServiceNode.ServiceStack.Attributes;

namespace Nwpie.MiniSite.Storage.Common.Attributes
{
    public class CustomTokenFilterAsyncAttribute : TokenFilterAsyncAttribute
    {
        public override async Task<bool?> IsValidAccount(ITokenDataModel tokenModel)
        {
            await Task.CompletedTask;
            return true == tokenModel?.GetAccountId().HasValue();
        }
    }
}
