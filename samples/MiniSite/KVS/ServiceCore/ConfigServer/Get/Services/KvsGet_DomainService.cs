using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Entities;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Get;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Services
{
    public class KvsGet_DomainService :
        DomainService,
        IKvsGet_DomainService
    {
        public async Task<KvsGet_ResponseModel> Execute(KvsGet_ParamModel param)
        {
            Validate(param);

            var result = new KvsGet_ResponseModel()
            {
                RawData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };

            var access = await GetAccessApiKey(param);
            foreach (var config in param?.ConfigKeys)
            {
                var possibleOwnerAppList = await access.PossibleOwnerAppList(config.FriendAppName);
                var valueList = await ConfigDomainHelper.GetValueList(config.ConfigKey,
                    config.Version.HasValue()
                        ? config.Version
                        : ConfigConst.LatestVersion,
                    possibleOwnerAppList,
                    computeDistance: true
                );

                if (false == valueList?.Count > 0)
                {
                    throw new Exception($"ConfigValue of {nameof(config.ConfigKey)} (={config.ConfigKey}) not exists. ");
                }

                valueList = valueList
                    ?.OrderByDescending(x => x.distance)
                    ?.ToList();

                var permissionList = (
                    await ConfigDomainHelper.GetConfigPermissionList(config.ConfigKey, possibleOwnerAppList)
                )?.OrderByDescending(x => x.weight);

                foreach (var value in valueList)
                {
                    if (config.FriendAppName.HasValue())
                    {
                        var friendApp = await ApplicationDomainHelper.GetApplicationByNameAsync(config.FriendAppName)
                            ?? throw new Exception($"Application of {nameof(config.FriendAppName)} (={config.FriendAppName}) not exists. ");

                        if (permissionList.IsAllowAccess(valueList, friendApp.app_id, access.AppId))
                        {
                            var friendApiKey = await ApiKeyDomainHelper.GetBaseApiKeyAsync(friendApp.app_id)
                                ?? throw new Exception($"ApiKey of {nameof(config.FriendAppName)} (={config.FriendAppName}) not exists. ");

                            var isChild = await ConfigDomainHelper.IsChildOf(value.apikey, friendApiKey.apikey);
                            if (isChild)
                            {
                                result.RawData.Add(value.configkey, value.DecryptData());
                                break;
                            }
                        }

                        continue;
                    }

                    if (permissionList.IsAllowAccess(valueList, value.app_id, access.AppId))
                    {
                        var isChild = await ConfigDomainHelper.IsChildOf(value.apikey, access.Apikey);
                        if (isChild)
                        {
                            result.RawData.Add(value.configkey, value.DecryptData());
                            break;
                        }
                    }
                }
            }

            if (result.RawData?.Count() > 0)
            {
                return result;
            }

            return null;
        }

        public bool Validate(KvsGet_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
