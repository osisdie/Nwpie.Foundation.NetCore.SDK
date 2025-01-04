using Nwpie.Foundation.Abstractions.Auth.Enums;

namespace Nwpie.Foundation.Auth.SDK.Utilities
{
    public static class PermissionUtils
    {
        #region converter
        public static PermissionTypeEnum ConvertToPermissionType(string type, bool? blocked)
        {
            switch (type)
            {
                case "app_id":
                    return true == blocked
                        ? PermissionTypeEnum.DenyAppId
                        : PermissionTypeEnum.AllowAppId;
                case "apikey":
                    return true == blocked
                        ? PermissionTypeEnum.DenyApiKey
                        : PermissionTypeEnum.AllowApiKey;
                case "group_id":
                    return true == blocked
                        ? PermissionTypeEnum.DenyGroupId
                        : PermissionTypeEnum.AllowGroupId;
                case "account_id":
                    return true == blocked
                        ? PermissionTypeEnum.DenyAccountId
                        : PermissionTypeEnum.AllowAccountId;
                default:
                    return PermissionTypeEnum.UnSet;
            };
        }
        #endregion
    }
}
