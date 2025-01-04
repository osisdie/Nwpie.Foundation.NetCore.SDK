using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.Foundation.Storage.S3.Interfaces;
using ServiceStack;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class SimpleEchoTest_Service : ApiServiceAnyOutputEntry<
        SimpleEchoTest_Request,
        SimpleEchoTest_RequestModel,
        string,
        SimpleEchoTest_ParamModel,
        ISimpleEchoTest_DomainService>

    {
        public override void OnValidationProcessBegin(SimpleEchoTest_Request request)
        {
            if (true == request?.Data?.RequestString?.Contains("exception"))
            {
                throw new Exception("Mannual exception. ");
            }
        }

        public override ISimpleEchoTest_DomainService ResolveService() =>
            new SimpleEchoTest_DomainService();
    }

    public interface ISimpleEchoTest_DomainService : IDomainService
    {
        Task<string> Execute(SimpleEchoTest_ParamModel param);
    }

    public class SimpleEchoTest_DomainService :
        DomainServiceBase,
        ISimpleEchoTest_DomainService
    {
        public async Task<string> Execute(SimpleEchoTest_ParamModel param)
        {
            Validate(param);

            await Task.CompletedTask;
            return param.RequestString;
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

        public bool Validate(SimpleEchoTest_RequestModel param)
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
    public class SimpleEchoTest_Request :
        ContractRequestBase<SimpleEchoTest_RequestModel>,
        IRequestDataNotAllowDefault,
        IServiceReturn<string>
    {

    }

    #endregion

    public class SimpleEchoTest_RequestModel : RequestDtoBase
    {
        /// <summary>
        /// SimpleEcho words
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [ApiMember(Description = "Echo words", IsRequired = true)]
        public string RequestString { get; set; }
    }
    public class SimpleEchoTest_ParamModel : SimpleEchoTest_RequestModel
    {

    }
}
