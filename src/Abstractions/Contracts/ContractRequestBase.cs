using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Abstractions.Contracts
{
    public class ContractRequestBase : ContractRequestBase<object>
    {
    }

    //public class ContractRequestBase<T> where T : ParamModelBase, new()
    //public class ContractRequestBase<TRequestDto> : ContractRequest<TRequestDto>
    //    where TRequestDto : RequestDtoBase, new()
    public class ContractRequestBase<T> : RequestDataModel<T>
    {
    }
}
