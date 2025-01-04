using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.MiniSite.Storage.Common.Services;
using Nwpie.MiniSite.Storage.Contract.Test;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.ServiceCore.Test
{
    public class Test_DomainService :
        DomainService,
        ITest_DomainService
    {
        public async Task<Test_Response> Execute(Test_Request param)
        {
            Validate(param);

            var response = Test_DomainConverter.ConvertToResponseModel(param);
            response.Requester = GetRequester();

            //var svc1 = GetDomainService<IDamExists_DomainService>();
            //var svc2 = GetDomainService<IDamExists_DomainService>();
            //response.SubMsg = $"Object.ReferenceEquals(svc1,svc2) == {Object.ReferenceEquals(svc1, svc2)}";
            response.Success(StatusCodeEnum.EmptyData);

            await Task.CompletedTask;
            return response;
        }

        public bool Validate(Test_Request param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
