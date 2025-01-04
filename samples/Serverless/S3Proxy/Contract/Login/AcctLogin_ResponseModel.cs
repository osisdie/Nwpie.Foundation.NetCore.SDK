using System;
using Nwpie.Foundation.Abstractions.Contracts.Models;

namespace Nwpie.Foundation.S3Proxy.Contract.Login
{
    public class AcctLogin_ResponseModel : ResultDtoBase
    {
        #region Account Entity
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? Updated { get; set; }

        #endregion

        /// <summary>
        /// Obsolete token
        /// </summary>
        [Obsolete("Use AccessToken")]
        public string Token { get; set; }

        /// <summary>
        /// access_token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// refresh_token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
