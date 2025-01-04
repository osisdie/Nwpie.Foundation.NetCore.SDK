using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.Notification.Endpoint.Common
{
    public abstract class CustomServiceEntryBase<
        T_Request,
        T_Response,
        T_Request_DataModel,
        T_Response_DataModel,
        T_DomainService_ParamModel,
        T_DomainService> : ApiServiceEntry<
            T_Request,
            T_Response,
            T_Request_DataModel,
            T_Response_DataModel,
            T_DomainService_ParamModel,
            T_DomainService>
        where T_Request : class, IRequestMessage<T_Request_DataModel> // ContractRequestBase<T_Request_DataModel>
        where T_Response : IServiceResponse<T_Response_DataModel>, new() // ContractResponseBase<T_Response_DataModel>, new()
        where T_DomainService : class, IDomainService
        where T_Request_DataModel : class, IRequestDto // RequestDtoBase
        //where T_Response_DataModel : class, IResultDto, new() // ResultDtoBase
        where T_DomainService_ParamModel : class, IRequestDto // RequestDtoBase
    {

    }
}
