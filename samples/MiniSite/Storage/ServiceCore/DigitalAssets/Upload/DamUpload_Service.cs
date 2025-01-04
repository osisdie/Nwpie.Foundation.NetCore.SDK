using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.Storage.Common.Attributes;
using Nwpie.MiniSite.Storage.Contract.Assets.Upload;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Upload.Interfaces;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Upload
{
    [CustomTokenFilterAsync]
    public class DamUpload_Service : ApiServiceAnyInOutEntry<
    DamUpload_Request,
    DamUpload_Response,
    IDamUpload_DomainService>
    {
        public override void OnValidationProcessEnd(bool isValid, string errMsg = null)
        {
            if (null != errMsg)
            {
                ContractResponseDto?.Error(StatusCodeEnum.InvalidContractRequest, errMsg);
            }
        }
    }
}
