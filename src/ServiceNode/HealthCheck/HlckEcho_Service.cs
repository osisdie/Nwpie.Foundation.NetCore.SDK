using System;
using Nwpie.Foundation.ServiceNode.HealthCheck.Contracts;
using Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces;
using Nwpie.Foundation.ServiceNode.HealthCheck.Models;
using Nwpie.Foundation.ServiceNode.HealthCheck.Services;

namespace Nwpie.Foundation.ServiceNode.HealthCheck
{
    public class HlckEcho_Service : HttpService<
    HlckEcho_Request,
    HlckEcho_Response,
    HlckEcho_RequestModel,
    HlckEcho_ResponseModel,
    HlckEcho_ParamModel,
    IHlckEcho_DomainService>
    {
        public HlckEcho_Service(IHlckEcho_DomainService domainService)
        {
            DomainService = domainService;
        }

        public override void OnValidationProcessBegin(HlckEcho_Request request)
        {
            if (true == request.Data?.RequestString?.Contains("exception"))
            {
                throw new Exception("Mannual exception. ");
            }
        }
    }
}
