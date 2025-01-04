using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Entities;
using Nwpie.MiniSite.KVS.Common.Models;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Common.Utilities;
using Nwpie.MiniSite.KVS.Contract.Configserver.Permission;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Services
{
    public class KvsPermission_DomainService :
        DomainService,
        IKvsPermission_DomainService
    {
        public async Task<KvsPermission_ResponseModel> Execute(KvsPermission_ParamModel param)
        {
            Validate(param);

            var result = new KvsPermission_ResponseModel();
            var access = await GetAccessApiKey(param);
            await GrantRead(param, result, access);
            await GrantWrite(param, result, access);

            if (0 == result.CountFailed && (result.CountInserted > 0 || result.CountUpdated > 0))
            {
                return result;
            }

            return null;
        }

        private async Task GrantWrite(KvsPermission_ParamModel param, KvsPermission_ResponseModel result, KvsApiKey access)
        {
            if (AccessLevelEnum.Any == param.Permission.AllowWrite_Behavior &&
                true != param.Permission.AllowWrite_ApiNames?.Count() > 0)
            {
                var apikeyList = await ApiKeyDomainHelper.ListApiKeyAsync();
                param.Permission.AllowWrite_ApiNames = apikeyList
                    .Where(o => o.env == Enum<EnvironmentEnum>.GetDispValue(EnvironmentEnum.Testing))
                    .Select(o => o.sys_name)
                    .ToList();
            }

            if (param?.Permission.AllowWrite_ApiNames?.Count() > 0)
            {
                var res = await Grant(access,
                    param,
                    param.Permission.AllowWrite_ApiNames,
                    PermissionLevelEnum.Update
                );

                if (null != res)
                {
                    result.CountDeleted += res.CountDeleted;
                    result.CountFailed += res.CountFailed;
                    result.CountInserted += res.CountInserted;
                    result.CountUpdated += res.CountUpdated;
                }
            }
        }

        private async Task GrantRead(KvsPermission_ParamModel param, KvsPermission_ResponseModel result, KvsApiKey access)
        {
            if (AccessLevelEnum.Any == param.Permission.AllowRead_Behavior &&
                true != param.Permission.AllowRead_ApiNames?.Count() > 0)
            {
                var apikeyList = await ApiKeyDomainHelper.ListApiKeyAsync();
                param.Permission.AllowRead_ApiNames = apikeyList
                    .Where(o => o.env == Enum<EnvironmentEnum>.GetDispValue(EnvironmentEnum.Testing))
                    .Select(o => o.sys_name).ToList();
            }

            if (param.Permission.AllowRead_ApiNames?.Count() > 0)
            {
                var res = await Grant(access,
                    param,
                    param.Permission.AllowRead_ApiNames,
                    PermissionLevelEnum.Query
                );

                if (null != res)
                {
                    result.CountDeleted += res.CountDeleted;
                    result.CountFailed += res.CountFailed;
                    result.CountInserted += res.CountInserted;
                    result.CountUpdated += res.CountUpdated;
                }
            }
        }

        private async Task<bool> IsAllowGrant(KvsApiKey access, KvsPermission_RequestModel requestDto)
        {
            // 1. Config key exists AND
            // 1.1. Grant self App config => pass, OR
            // 1.2. Grant platform app config => pass if current apikey is nwpie or debug mode
            //
            var possibleOwnerAppList = await access.PossibleOwnerAppList(requestDto.FriendAppName);

            var valueList = await ConfigDomainHelper.GetValueList(requestDto.ConfigKey,
                ConfigConst.LatestVersion,
                possibleOwnerAppList,
                computeDistance: false
            );

            if (false == valueList?.Count > 0)
            {
                throw new Exception($"ConfigKey (={requestDto.ConfigKey}) not exists. ");
            }

            var isSelfConfig = valueList.Any(x => x.app_id == access.AppId);
            if (isSelfConfig)
            {
                return true;
            }

            var isPlatformConfig = valueList.Any(x => x.app_id == KvsConfigUtils.PlatformAppId);
            if (isPlatformConfig)
            {
                if (access.Env == Enum<EnvironmentEnum>.GetDispValue(EnvironmentEnum.Debug) ||
                    access.IsPlatformApikey())
                {
                    return true;
                }
            }

            throw new Exception($"Permission denied to grant configKey (={requestDto.ConfigKey}) permission. ");
        }

        private async Task<KvsPermission_ResponseModel> Grant(KvsApiKey access, KvsPermission_RequestModel requestDto, List<string> apiNameList, PermissionLevelEnum level)
        {
            if (false == apiNameList?.Count > 0)
            {
                return null;
            }

            await IsAllowGrant(access, requestDto);

            var result = new KvsPermission_ResponseModel();
            var isPlatformConfigKey = null != await ConfigDomainHelper.GetLatestValue(requestDto.ConfigKey,
                KvsConfigUtils.PlatformAppId,
                KvsConfigUtils.PlatformBaseApiKey,
                computeDistance: false
            );

            var groups = apiNameList?.GroupBy(api => (
                ApiKeyDomainHelper.GetApiKeyByNameAsync(api).ConfigureAwait(false).GetAwaiter().GetResult()
            )?.app_id);

            foreach (var group in groups)
            {
                if (true != group?.Key.HasValue())
                {
                    throw new Exception($"Invalid ApiName in (={string.Join(",", apiNameList)} ). ");
                }

                var possibleOwnerAppList = await access.PossibleOwnerAppList(requestDto.FriendAppName);
                var ownerAppId = isPlatformConfigKey
                    ? KvsConfigUtils.PlatformAppId
                    : access.AppId;
                if (requestDto.FriendAppName.HasValue())
                {
                    var app = await ApplicationDomainHelper.GetApplicationByNameAsync(requestDto.FriendAppName);
                    ownerAppId = app?.app_id;
                }

                var param = new PERMISSION_CONFIG_KEY_Entity
                {
                    app_id = ownerAppId,
                    target_id = group.Key,
                    target_type = Enum<PermissionTypeEnum>.GetDispValue(PermissionTypeEnum.AllowAppId),
                    blocked = level == PermissionLevelEnum.Deny
                };

                await param.SetPermissionWeight(level);

                var setResult = await ConfigDomainHelper.SetPermission(requestDto.ConfigKey, param);
                if (setResult.HasValue)
                {
                    _ = setResult > 0
                        ? ++result.CountInserted
                        : ++result.CountUpdated;
                }
                else
                {
                    ++result.CountFailed;
                }
            }

            return result;
        }

        public bool Validate(KvsPermission_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
