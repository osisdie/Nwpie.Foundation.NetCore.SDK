namespace Nwpie.Foundation.Abstractions.Contracts
{
    public class ContractResponseBase : ContractResponseBase<object>
    {

    }

    //public class ContractResponseBase<T> : ResponseBase where T : DomainData
    public class ContractResponseBase<T> : ServiceResponse<T>
    {
        public ContractResponseBase() : base() { }
        public ContractResponseBase(bool isSuccess) : base(isSuccess) { }
    }
}
