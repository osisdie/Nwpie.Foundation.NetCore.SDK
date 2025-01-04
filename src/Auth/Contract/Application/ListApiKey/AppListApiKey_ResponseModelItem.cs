using System;
using System.Runtime.Serialization;

namespace Nwpie.Foundation.Auth.Contract.Application.ListApiKey
{
    public class AppListApiKey_ResponseModelItem
    {
        #region special case for ApiKey
        public string ApiKey { get; set; }
        public string TokenName { get; set; }

        [Obsolete]
        public string AppId { get; set; }
        [Obsolete]
        public string ApiName { get; set; }
        [Obsolete]
        public string SysName { get; set; }
        [Obsolete]
        public string Env { get; set; }
        #endregion

        public string AccountId { get; set; }
        public string Name { get; set; } // AppName (NOT ApiName !!)
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public string Status { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }

        [IgnoreDataMember]
        public string CredentialKey { get; set; }

        [IgnoreDataMember]
        public byte? CredentialKind { get; set; }
    }
}
