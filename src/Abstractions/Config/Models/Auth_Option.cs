namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Auth_Option : OptionBase
    {
        public bool Enabled { get; set; }
        public string AdminToken { get; set; }
        public bool AdminTokenEnabled { get; set; }
        public int CacheMinutes { get; set; }
        public int ExpireMinutes { get; set; }
        public int AccessTokenMinutes { get; set; }
        public int RefreshTokenMinutes { get; set; }
        public int ApiTokenMinutes { get; set; }

        /// <summary>
        /// Required token's version (bundle in Creator) >= this value
        /// </summary>
        public int MinimumAccessTokenVersion { get; set; }
        public int MinimumRefershTokenVersion { get; set; }
        public int MinimumApiTokenVersion { get; set; }

        /// <summary>
        /// @see JwtHashAlgorithmEnum
        /// </summary>
        public string JwtAlgorithm { get; set; }
        public string JwtPrivateKey { get; set; }

        /// <summary>
        /// RsaParameters.ToPrivateKeyXml
        /// </summary>
        public string JwtPrivateKeyXml { get; set; }

        /// <summary>
        /// RsaParameters.ToPublicKeyXml
        /// </summary>
        public string JwtPublicKeyXml { get; set; }

        /// <summary>
        /// Convert.ToBase64String(AesUtil.CreateKey())
        /// </summary>
        public string JwtAuthKeyBase64 { get; set; }

        public string AuthAESKey { get; set; }
        public string AuthAESIV { get; set; }
    }
}
