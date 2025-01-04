using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Abstractions.Contracts
{
    public class ServiceResponse : ResponseBase, IServiceResponse
    {
        public ServiceResponse() { }
        public ServiceResponse(bool isSuccess)
        {
            _ = isSuccess ? this.Success() : this.Error();
        }

        public ServiceResponse(IServiceResponse res)
        {
            if (null != res)
            {
                Code = res.Code;
                Msg = res.Msg;
                ErrMsg = res.ErrMsg;
                SubCode = res.SubCode;
                SubMsg = res.SubMsg;
                IsSuccess = res.IsSuccess;
                //ExtendedDictionary = res.ExtendedDictionary;
            }
        }
    }

    /// <summary>
    /// Usage: The generic type could be primitive type or object type, to store actual return data
    ///         Wraper a ResponseBase model so we could give extra information, such as IsSuccess, StatusCode, etc
    /// Example:
    ///	    ServiceResponse<bool>
    ///	    ServiceResponse<Account>
    ///	    ServiceResponse<Dictionary<string,string>>
    /// </summary>
    public class ServiceResponse<T> : ResponseBase, IServiceResponse<T>
    {
        public ServiceResponse() { }

        public ServiceResponse(bool isSuccess)
        {
            _ = isSuccess ? this.Success() : this.Error();
        }

        public ServiceResponse(bool isSuccess, T data) : this(isSuccess)
        {
            Data = data;
        }

        public ServiceResponse(IServiceResponse res)
        {
            if (null != res)
            {
                Code = res.Code;
                Msg = res.Msg;
                ErrMsg = res.ErrMsg;
                SubCode = res.SubCode;
                SubMsg = res.SubMsg;
                IsSuccess = res.IsSuccess;
                //ExtendedDictionary = res.ExtendedDictionary;
            }
        }

        public T Data { get; set; }
    }
}
