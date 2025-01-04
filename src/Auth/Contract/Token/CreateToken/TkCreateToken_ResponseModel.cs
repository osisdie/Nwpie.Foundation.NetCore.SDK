using System;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Token.CreateToken
{
    public class TkCreateToken_ResponseModel : ResultDtoBase
    {
        public string AccountId { get; set; }
        public string AccessKey { get; set; } // ApiKey, Bearer Token, PAT
        public string Name { get; set; }
        public string Env { get; set; }
        public byte? Kind { get; set; }
        public byte? Level { get; set; }
        public string Description { get; set; }
        public DateTime? Expired { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
    }
}
