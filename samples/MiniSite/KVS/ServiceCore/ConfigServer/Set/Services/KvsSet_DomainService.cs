using System;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Common.Utilities;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Services
{
    public class KvsSet_DomainService :
        DomainService,
        IKvsSet_DomainService
    {
        public async Task<KvsSet_ResponseModel> Execute(KvsSet_ParamModel param)
        {
            Validate(param);

            var result = new KvsSet_ResponseModel();
            var access = await GetAccessApiKey(param);
            if (param.NeedEncrypt)
            {
                param.RawData = CryptoUtils.EncodeToAES(param.RawData, KvsConfigUtils.AuthOption.AuthAESKey, KvsConfigUtils.AuthOption.AuthAESIV);
            }

            var ownerAppId = access.AppId;
            var inUseApiKey = param.IsBaseConfig
                ? access.BaseApiKey().apikey
                : access.Apikey;
            var possibleOwnerAppList = await access.PossibleOwnerAppList();
            if (false == access.IsPlatformApikey())
            {
                var valueList = await ConfigDomainHelper.GetValueList(param.ConfigKey,
                    ConfigConst.LatestVersion,
                    possibleOwnerAppList,
                    computeDistance: false
                );
                var isPlatformConfig = valueList.Any(x => x.app_id == KvsConfigUtils.PlatformAppId);
                if (isPlatformConfig)
                {
                    if (access.IsPlatformApikey() ||
                        access.Env == Enum<EnvironmentEnum>.GetDispValue(EnvironmentEnum.Debug))
                    {
                        ownerAppId = KvsConfigUtils.PlatformAppId;
                    }
                    else
                    {
                        throw new Exception($"Permission denied to set platform config (={param.ConfigKey}). ");
                    }
                }
            }

            var configKey = await ConfigDomainHelper.HasConfigKey(param.ConfigKey, possibleOwnerAppList);
            if (true != configKey)
            {
                if (param.ConfigSection.IsNullOrEmpty())
                {
                    var splits = param.ConfigKey.Split(new char[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    splits.RemoveAt(splits.Count() - 1);
                    param.ConfigSection = splits.Count() <= 1
                        ? string.Empty
                        : string.Join(ConfigConst.GoNextHierarchyDivider, splits);
                }

                var configKeyEntity = await ConfigDomainHelper.NewConfigKey(ownerAppId, param)
                    ?? throw new Exception($"Failed to create {nameof(param.ConfigKey)} (={param.ConfigKey}). ");
            }

            var newValue = await ConfigDomainHelper.SetConfig(ownerAppId, inUseApiKey, param);
            if (null != newValue)
            {
                result.VersionDisplay = newValue.version_display;
                result.VersionTimeStamp = DateTime.UtcNow.ToString("s");
                return result;
            }

            return null;
        }

        public bool Validate(KvsSet_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
