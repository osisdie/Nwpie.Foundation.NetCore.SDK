using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Auth.Contract
{
    public sealed class AuthIdentifierHelper
    {
        public static string NewId() => IdentifierUtils.NewId();

        #region Account
        public const string AccountPrefix = "m";
        public static string NewIdForAccount() => $"{AccountPrefix}{__}{_ENV_}{__}{NewId()}";

        public const string PasswordPrefix = "pw";
        public static string NewIdForPassword() => $"{PasswordPrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region Role
        public const string RolePrefix = "r";
        public static string NewIdForRole() => $"{RolePrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region App
        public const string AppPrefix = "app";
        public static string NewIdForApplication() => $"{AppPrefix}{__}{_ENV_}{__}{NewId()}";

        public const string ApiKeyPrefix = "api";
        public static string NewIdForApiKey() => $"{ApiKeyPrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region Group
        public const string GroupPrefix = "grp";
        public static string NewIdForGroup() => $"{GroupPrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region Module
        public const string ModulePrefix = "mdl";
        public static string NewIdForModule() => $"{ModulePrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region Permission
        public const string ResourcePrefix = "res";
        public static string NewIdForResource() => $"{ResourcePrefix}{__}{_ENV_}{__}{NewId()}";

        public const string PermissionPrefix = "prm";
        public static string NewIdForPermission() => $"{PermissionPrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region Policy
        public const string PolicyPrefix = "plcy";
        public static string NewIdForPolicy() => $"{PolicyPrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        #region Token (PAT)
        public const string TokenPrefix = "tk";
        public static string NewIdForToken() => $"{TokenPrefix}{__}{_ENV_}{__}{NewId()}";

        #endregion

        private const string __ = "_"; // divider
        private static readonly string _ENV_ = (SdkRuntime.SdkEnv?[0] ?? 'd').ToString();
    }
}
