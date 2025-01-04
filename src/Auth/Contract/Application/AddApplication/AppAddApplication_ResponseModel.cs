using System;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.Auth.Contract.Application.AddApplication
{
    public class AppAddApplication_ResponseModel : ResultDtoBase
    {
        public string AppId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public AppAddApiKey_ResponseModel ApiKeyInfo { get; set; }
    }
}
