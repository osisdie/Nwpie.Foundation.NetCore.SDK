namespace Nwpie.Foundation.Auth.Contract.Account.GetProfile
{
    public class AcctGetProfile_RequestModel : AuthParamModel
    {
        #region optional
        public override string AccountId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        #endregion
    }
}
