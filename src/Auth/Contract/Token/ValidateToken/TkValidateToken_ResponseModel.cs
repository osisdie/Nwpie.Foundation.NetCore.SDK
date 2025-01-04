using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Auth.Contract.Account.GetProfile;

namespace Nwpie.Foundation.Auth.Contract.Token.ValidateToken
{
    public class TkValidateToken_ResponseModel : ResultDtoBase
    {
        public AcctGetProfile_ResponseModel Profile { get; set; }
    }
}
