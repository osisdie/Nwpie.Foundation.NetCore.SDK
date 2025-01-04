using System;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Account.GetProfile
{
    public class AcctGetProfile_ResponseModel : ResultDtoBase,
        IAccountProfile
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; } // json
        public string Metadata { get; set; } // json
        [Obsolete]
        public string Env { get; set; }
        [Obsolete]
        public string SysName { get; set; } // Obsolete apiname
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
