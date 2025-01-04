using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Statics;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.Contract.Test
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("Storage:Test")]
    [Route("/Test", "GET,POST")]
    public class Test_Request :
        ContractRequestBase, //<Test_RequestModel>,
        IServiceReturn<Test_Response>
    {
        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [RegularExpression(ConfigConst.PasswordPattern)]
        [ApiMember(IsRequired = true)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        [ApiMember(IsRequired = true)]
        public string ConfirmPassword { get; set; }

        public string Echo { get; set; }
    }

    public class Test_RequestModel : RequestDtoBase
    {
        [Required]
        [MinLength(ConfigConst.MinPasswordLength)]
        [RegularExpression(ConfigConst.PasswordPattern)]
        [ApiMember(IsRequired = true)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        [ApiMember(IsRequired = true)]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class Test_Response :
        ContractResponseBase//<Test_ResponseModel>
    {
        public string Requester { get; set; }
        public string EchoBack { get; set; }
    }

    public class Test_ResponseModel : ResultDtoBase
    {

    }
    #endregion
}
