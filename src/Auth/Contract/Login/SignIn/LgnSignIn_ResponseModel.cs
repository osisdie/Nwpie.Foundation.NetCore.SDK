using Nwpie.Foundation.Auth.Contract.Account.GetProfile;

namespace Nwpie.Foundation.Auth.Contract.Login.SignIn
{
    public class LgnSignIn_ResponseModel : AcctGetProfile_ResponseModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
