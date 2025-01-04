using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nwpie.Foundation.Auth.Contract.Account.ListAccount
{
    public class AcctListAccount_RequestModel : AuthParamModel
    {
        public string ParentAccountId { get; set; }
        public List<string> AccountIds { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public List<string> Tags { get; set; }
        public string Status { get; set; }
        public List<string> StatusIncluded { get; set; }
        public List<string> StatusExcluded { get; set; }

        #region Hidden fields
        [IgnoreDataMember]
        public List<string> CredentialKeys { get; set; }

        #endregion
    }
}
