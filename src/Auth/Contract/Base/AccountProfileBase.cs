using System;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;

namespace Nwpie.Foundation.Auth.Contract.Base
{
    public class AccountProfileBase : IAccountProfile
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Tags { get; set; } // json
        public string Metadata { get; set; } // json
        public DateTime? LastLoginDate { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }

        [Obsolete]
        public string SysName { get; set; }
        [Obsolete]
        public string Env { get; set; }

        [IgnoreDataMember]
        public DateTime? _ts { get; set; }
    }
}
