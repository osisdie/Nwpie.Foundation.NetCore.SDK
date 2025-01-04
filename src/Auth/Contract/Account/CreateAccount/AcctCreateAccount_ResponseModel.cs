using System;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Account.CreateAccount
{
    public class AcctCreateAccount_ResponseModel : ResultDtoBase
    {
        public string CredentialKey { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public DateTime? Expired { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
    }
}
