using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nwpie.Foundation.Auth.Contract.Application.ListApiKey
{
    public class AppListApiKey_RequestModel : AuthParamModel
    {
        public string AppId { get; set; } // App AccountId
        public string AccessKey { get; set; } // ApiKey
        public string Name { get; set; } // AppName
        public string TokenName { get; set; } // AppName.Env
        public byte? Kind { get; set; }

        #region Hidden fields
        [IgnoreDataMember]
        public List<string> CredentialKeys { get; set; }

        #endregion
    }
}
