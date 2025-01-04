using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
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
    public class SimpleStringEchoTest_Service : ApiServiceAnyInOutEntry<
        SimpleStringEchoTest_Request,
        string,
        ISimpleStringEchoTest_DomainService>

    {
        public override void OnValidationProcessBegin(SimpleStringEchoTest_Request request)
        {
            if (true == request?.RequestString?.Contains("exception"))
            {
                throw new Exception("Mannual exception. ");
            }
        }

        public override ISimpleStringEchoTest_DomainService ResolveService() =>
            new SimpleStringEchoTest_DomainService();
    }

    public interface ISimpleStringEchoTest_DomainService : IDomainService
    {
        Task<string> Execute(SimpleStringEchoTest_Request param);
    }

    public class SimpleStringEchoTest_DomainService :
        DomainServiceBase,
        ISimpleStringEchoTest_DomainService
    {
        public async Task<string> Execute(SimpleStringEchoTest_Request param)
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

        public bool Validate(SimpleStringEchoTest_Request param)
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
    public class SimpleStringEchoTest_Request :
        SimpleStringEchoTest_RequestModel,
        IServiceReturn<string>
    {

    }

    #endregion

    public class SimpleStringEchoTest_RequestModel : RequestDtoBase
    {
        /// <summary>
        /// SimpleStringEcho words
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [ApiMember(Description = "Echo words", IsRequired = true)]
        public string RequestString { get; set; }
    }
}
