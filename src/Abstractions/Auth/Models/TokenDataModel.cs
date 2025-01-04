using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Abstractions.Auth.Models
{
    public class TokenDataModel : ITokenDataModel
    {
        //[Obsolete]
        //public string Email { get; set; }

        /// <summary>
        /// Account GUID
        /// * The token stands for an end-user role
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Which application user logon
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// ApiKey in an application
        /// * The token stands for an application role
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The application or end-user's displayname
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// X-Forwarded-For.Split(',').First()
        /// X-Real-IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Hashed User-Agent
        /// </summary>
        public string UA { get; set; }

        /// <summary>
        /// Mobile deviceId
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// Mac address
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// Is mobile device ?
        /// If yes, don't need to detect IP changes
        /// </summary>
        public bool? Mob { get; set; }

        /// <summary>
        /// @see TokenKindEnum
        /// </summary>
        public byte? Kind { get; set; }

        /// <summary>
        /// @see TokenLevelEnum
        /// </summary>
        public byte? LV { get; set; }

        /// <summary>
        /// Update Timestamp
        /// Init: DateTime.UtcNow
        /// Refresh: DateTime.UtcNow
        /// </summary>
        public DateTime? Upt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Service name who creates the token
        /// </summary>
        public string Iss { get; set; }

        /// <summary>
        /// Expiration Timestamp
        /// Init: DateTime.UtcNow + token length
        /// Refresh: DateTime.UtcNow + token length
        /// </summary>
        public DateTime? Exp { get; set; }

        /// <summary>
        /// Creation timestamp (DateTime.UtcNow)
        /// </summary>
        public DateTime? Iat { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Token GUID
        /// </summary>
        public string _id { get; set; } = IdentifierUtils.NewId();

        /// <summary>
        /// Additional Key-Value pairs infomation
        /// </summary>
        public Dictionary<string, string> ExtensionMap { get; set; }
    }
}
