using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nwpie.Foundation.Auth.Contract.Account.UptProfile
{
    public class AcctUptProfile_RequestModel : AuthParamModel
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool? SetInActive { get; set; }

        public List<string> Tags { get; set; }
        public Dictionary<string, string> Profile { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        #region Admin, hidden fields
        [IgnoreDataMember]
        public string Status { get; set; }
        #endregion
    }
}
