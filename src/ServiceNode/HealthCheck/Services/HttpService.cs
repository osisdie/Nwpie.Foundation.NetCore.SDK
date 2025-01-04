using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Services
{
    public abstract class HttpService<
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
        where T_Request : ContractRequestBase<T_Request_DataModel>
        where T_Response : ContractResponseBase<T_Response_DataModel>, new()
        where T_DomainService : class, IDomainService
        where T_Request_DataModel : RequestDtoBase, new()
        where T_Response_DataModel : ResultDtoBase, new()
        where T_DomainService_ParamModel : RequestDtoBase
    {

    }
}
