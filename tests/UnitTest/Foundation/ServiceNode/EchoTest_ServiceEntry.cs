using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.Foundation.Storage.S3.Interfaces;
using ServiceStack;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class EchoTest_Service : ApiServiceEntry<
        EchoTest_Request,
        EchoTest_Response,
        EchoTest_RequestModel,
        EchoTest_ResponseModel,
        EchoTest_ParamModel,
        IEchoTest_DomainService>
    {
        public override void OnValidationProcessBegin(EchoTest_Request request)
        {
            if (true == request?.Data?.RequestString?.Contains("exception"))
            {
                throw new Exception("Mannual exception. ");
            }
        }

        public override IEchoTest_DomainService ResolveService() =>
            new EchoTest_DomainService();
    }

    public interface IEchoTest_DomainService : IDomainService
    {
        Task<EchoTest_ResponseModel> Execute(EchoTest_ParamModel param);
    }

    public class EchoTest_DomainService :
        DomainServiceBase,
        IEchoTest_DomainService
    {
        public async Task<EchoTest_ResponseModel> Execute(EchoTest_ParamModel param)
        {
            Validate(param);

            await Task.CompletedTask;
            return new EchoTest_ResponseModel()
            {
                ResponseString = param.RequestString
            };
        }

        public override ICache GetCache() =>
            ComponentMgr.Instance.TryResolve<ICache>();

        public override IStorage GetStorage() =>
            ComponentMgr.Instance.TryResolve<IStorage>() ??
            ComponentMgr.Instance.TryResolve<IS3StorageClient>();

        public override T GetDomainService<T>(bool isSelfService = false) =>
            ComponentMgr.Instance.TryResolve<T>();

        public override T GetRepository<T>(bool isSelfService = false) =>
            ComponentMgr.Instance.TryResolve<T>();

        public bool Validate(EchoTest_RequestModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }

    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Echo example")]
    [Route("/HealthCheck/Echo", "GET,POST")]
    [Route("/HealthCheck/HlckEchoRequest", "GET,POST", Notes = "Obsolete")]
    public class EchoTest_Request :
        ContractRequestBase<EchoTest_RequestModel>,
        IRequestDataNotAllowDefault,
        IServiceReturn<EchoTest_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class EchoTest_Response :
        ContractResponseBase<EchoTest_ResponseModel>
    {

    }
    #endregion

    public class EchoTest_RequestModel : RequestDtoBase
    {
        /// <summary>
        /// Echo words
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [ApiMember(Description = "Echo words", IsRequired = true)]
        public string RequestString { get; set; }
    }

    public class EchoTest_ParamModel : EchoTest_RequestModel
    {

    }

    public class EchoTest_ResponseModel : ResultDtoBase
    {
        /// <summary>
        /// Echo Response (Includes request words)
        /// </summary>
        [ApiMember(Description = "Echo Response (Includes request words)")]
        public string ResponseString { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        [ApiMember(Description = "Version")]
        public string Version { get; set; }

        public override bool Any() => ResponseString.HasValue();
    }
}
