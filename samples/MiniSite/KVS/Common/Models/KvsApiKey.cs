using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Common.Extras;
using Nwpie.MiniSite.KVS.Common.Domain;
using Nwpie.MiniSite.KVS.Common.Entities;
using Nwpie.MiniSite.KVS.Common.Utilities;

namespace Nwpie.MiniSite.KVS.Common.Models
{
    public class KvsApiKey : CObject
    {
        static KvsApiKey()
        {
            CacheClient = ComponentMgr.Instance.TryResolve<ICache>();
        }

        public KvsApiKey()
        {
        }

        public KvsApiKey(string apikey)
        {
            Apikey = apikey;
        }

        public Task<KvsApiKey> FillByApiKey(string apiKey)
        {
            Apikey = apiKey;
            return Fill();
        }

        public Task<KvsApiKey> FillByApiName(string apiName)
        {
            ApiName = apiName;
            return Fill();
        }

        protected async Task<KvsApiKey> Fill()
        {
            if (AppId.HasValue())
            {
                return this;
            }

            if (string.IsNullOrWhiteSpace(Apikey))
            {
                throw new ArgumentNullException(nameof(Apikey));
            }

            Entity = await ApiKeyDomainHelper.GetApiKeyByIdAsync(Apikey)
                ?? throw new Exception("Invalid ApiKey. ");

            ParentApikey = Entity.parent_apikey;
            Apikey = Entity.apikey;
            AppId = Entity.app_id;
            ApiName = Entity.sys_name;
            Env = Entity.env;
            Description = Entity.description;
            Status = Entity.status;
            Secretkey = Entity.secretkey;
            AccessLevel = Entity.access_level;
            StartDate = Entity.start_date;
            EndDate = Entity.end_date;

            return this;
        }

        public bool IsPlatformApplication() =>
            KvsConfigUtils.PlatformAppId == AppId;

        public bool IsPlatformApikey() =>
            KvsConfigUtils
                .PlatformApiKeyList
                ?.Any(o => o.apikey == Apikey)
                ?? false;

        public bool IsBaseApiKey() =>
            Env == Enum<EnvironmentEnum>.GetDispValue(EnvironmentEnum.Testing);

        public API_KEY_Entity BaseApiKey() =>
            IsBaseApiKey()
                ? Entity
                : ApiKeyDomainHelper
                    .GetBaseApiKeyByAppAsync(AppId).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<List<string>> PossibleOwnerAppList(string friendAppName = null)
        {
            var appList = new List<string>()
            {
                KvsConfigUtils.PlatformAppId
            };

            if (friendAppName.HasValue())
            {
                var friendApp = await ApplicationDomainHelper.GetApplicationByNameAsync(friendAppName)
                    ?? throw new Exception($"Invalid {nameof(friendAppName)}(={friendAppName}). ");

                appList.Add(friendApp.app_id);
            }
            else
            {
                appList.Add(AppId);
            }

            return appList;
        }

        public static ICache CacheClient { get; private set; }

        public string ParentApikey { get; set; }
        public string Apikey { get; set; }
        public string AppId { get; set; }
        public string ApiName { get; set; }
        public string Env { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Secretkey { get; set; }
        public int? AccessLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public API_KEY_Entity Entity { get; private set; }
    };
}
