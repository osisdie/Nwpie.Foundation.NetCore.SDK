using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Account.DeleteMe
{
    public class AcctDeleteMe_RequestModel : AuthParamModel
    {
        [Required]
        [ApiMember(IsRequired = true)]
        public bool? DeleteMe { get; set; }

        #region optional
        public string Note { get; set; }
        public override string AccountId { get; set; }
        #endregion
    }
}
