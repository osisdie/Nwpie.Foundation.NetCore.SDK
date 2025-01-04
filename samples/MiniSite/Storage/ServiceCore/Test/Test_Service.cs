using System.Net;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.Common.Attributes;
using Nwpie.MiniSite.Storage.Contract.Test;

namespace Nwpie.MiniSite.Storage.ServiceCore.Test
{
    [CustomTokenFilterAsync]
    public class Test_Service : ApiServiceAnyInOutEntry<
    Test_Request,
    Test_Response,
    ITest_DomainService>
    {
        public override void OnValidationProcessEnd(bool isValid, string errMsg = null)
        {
            if (false == isValid)
            {
                base.Response.StatusCode = (int)HttpStatusCode.OK;
                ContractResponseDto?.Error(StatusCodeEnum.ContractValidationError, errMsg);
            }
        }
    }
}
