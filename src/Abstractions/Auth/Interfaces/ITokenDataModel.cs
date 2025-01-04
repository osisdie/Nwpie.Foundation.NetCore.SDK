using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Auth.Interfaces
{
    public interface ITokenDataModel
    {
        /// <summary>
        /// Account GUID
        /// * The token stands for an end-user role
        /// </summary>
        string AccountId { get; set; }

        /// <summary>
        /// Original application's apiKey which user log-on
        /// </summary>
        string AppId { get; set; }

        /// <summary>
        /// Application GUID
        /// * The token stands for an application role
        /// </summary>
        string ApiKey { get; set; }

        /// <summary>
        /// The application or end-user's displayname
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// X-Forwarded-For.Split(',').First()
        /// X-Real-IP
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// Hashed User-Agent
        /// </summary>
        string UA { get; set; }

        /// <summary>
        /// Mobile deviceId
        /// </summary>
        string Device { get; set; }

        /// <summary>
        /// Mac address
        /// </summary>
        string Mac { get; set; }

        /// <summary>
        /// Is mobile device ?
        /// If yes, don't need to detect IP changes
        /// </summary>
        bool? Mob { get; set; }

        /// <summary>
        /// @see TokenKindEnum
        /// </summary>
        byte? Kind { get; set; }

        /// <summary>
        /// @see TokenLevelEnum
        /// </summary>
        byte? LV { get; set; }

        /// <summary>
        /// Update Timestamp
        /// Init: DateTime.UtcNow
        /// Refresh: DateTime.UtcNow
        /// </summary>
        DateTime? Upt { get; set; }

        /// <summary>
        /// Service name who created the token
        /// </summary>
        string Iss { get; set; }

        /// <summary>
        /// Expiration Timestamp
        /// Init: DateTime.UtcNow + token length
        /// Refresh: DateTime.UtcNow + token length
        /// </summary>
        DateTime? Exp { get; set; }

        /// <summary>
        /// Creation timestamp (DateTime.UtcNow)
        /// </summary>
        DateTime? Iat { get; set; }

        /// <summary>
        /// Token GUID
        /// </summary>
        string _id { get; set; }

        /// <summary>
        /// Additional Key-Value pairs infomation
        /// </summary>
        Dictionary<string, string> ExtensionMap { get; set; }
    }

    public static class ITokenDataModelExtension
    {
        /// <summary>
        /// Valid if ExpireAt > DateTime.UtcNow
        /// </summary>
        public static bool IsValid(this ITokenDataModel o) =>
            null != o.Exp && o.Exp > DateTime.UtcNow;
    }
}
