using System.ComponentModel.DataAnnotations;
using ServiceStack;

namespace Nwpie.Foundation.Auth.Contract.Application.UptApplication
{
    public class AppUptApplication_RequestModel : AuthParamModel
    {
        [Required]
        [ApiMember(IsRequired = true)]
        public string AppId { get; set; }

        #region optional
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
