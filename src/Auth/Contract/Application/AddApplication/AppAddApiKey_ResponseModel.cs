using System;
using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Application.AddApplication
{
    public class AppAddApiKey_ResponseModel : ResultDtoBase
    {
        public string ParentApikey { get; set; }
        public string AppId { get; set; }
        public string ApiKey { get; set; }
        public string ApiName { get; set; }
        public string Env { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? AccessLevel { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }

        [IgnoreDataMember]
        public string Secretkey { get; set; }

        [IgnoreDataMember]
        public bool? IsHidden { get; set; }
    }
}
