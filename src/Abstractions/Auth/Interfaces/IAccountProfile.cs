using System;
using System.Runtime.Serialization;

namespace Nwpie.Foundation.Abstractions.Auth.Interfaces
{
    public interface IAccountProfile
    {
        string AccountId { get; set; }
        string Name { get; set; }
        string DisplayName { get; set; }
        string Email { get; set; }
        string Description { get; set; }
        string Status { get; set; }
        string Tags { get; set; } // json
        string Metadata { get; set; } // json
        DateTime? LastLoginDate { get; set; }
        DateTime? Updated { get; set; }
        DateTime? Created { get; set; }

        [Obsolete]
        string SysName { get; set; }
        [Obsolete]
        string Env { get; set; }

        [IgnoreDataMember]
        DateTime? _ts { get; set; }
    }
}
