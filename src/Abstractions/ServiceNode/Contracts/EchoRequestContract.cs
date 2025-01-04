using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Abstractions.ServiceNode.Contracts
{
    public class EchoRequestContract :
        ContractRequestBase<EchoRequestModel>
    {

    }

    public class EchoResponseContract :
        ContractResponseBase<EchoRespnoseModel>
    {

    }

    public class EchoRequestModel : RequestDtoBase
    {
        public string Echo { get; set; }
    }

    public class EchoRespnoseModel :
        ResultDtoBase<KeyValuePair<string, string>>
    {

    }
}
