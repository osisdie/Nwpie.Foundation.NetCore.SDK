namespace Nwpie.Foundation.Auth.Contract.Login.SignOut
{
    public class LgnSignOut_RequestModel : AuthParamModel
    {
        public string AppId { get; set; }

        // Kick
        public override string AccountId { get; set; }
    }
}
