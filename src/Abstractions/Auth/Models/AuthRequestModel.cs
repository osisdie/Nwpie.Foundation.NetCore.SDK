namespace Nwpie.Foundation.Abstractions.Auth.Models
{
    /// <summary>
    /// Header: "Authorization"
    /// </summary>
    public class AuthRequestModel
    {
        /// <summary>
        /// Encrypted
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Original IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Mobile Mac address
        /// </summary>
        public string Mac { get; set; }
    }
}
