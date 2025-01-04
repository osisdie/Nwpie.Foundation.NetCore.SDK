using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location.Enums;
using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.Core;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.Foundation.Location.SDK
{
    public class LocationClient : CObject, ILocationClient
    {
        public LocationClient()
        {
            m_Serializer = new DefaultSerializer();

            if (ServiceContext.LocationServiceUrl.HasValue())
            {
                try
                {
                    SingleLocRequestPath = string.Concat(
                        ServiceContext.LocationServiceUrl.TrimEndSlash(),
                        typeof(LocGetApiLocation_Request).GetCustomAttributes<RouteAttribute>().FirstOrDefault().Path
                    );

                    BatchLocRequestPath = string.Concat(
                        ServiceContext.LocationServiceUrl.TrimEndSlash(),
                        typeof(LocBatchGetApiLocations_Request).GetCustomAttributes<RouteAttribute>().FirstOrDefault().Path
                    );

                    CreateBackgroudThreadToSyncLocation();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }
        }

        public async Task<string> GetApiUri(AppNameEnum appName)
        {
            var uri = string.Empty;

            try
            {
                var key = appName.ToString();
                if (m_ApiUriDic.TryGetValue(key, out uri))
                {
                    uri = m_ApiUriDic[key];
                }
                else
                {
                    uri = await GetApiLocation(key);
                    m_ApiUriDic.TryAdd(key, uri);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return uri;
        }

        public async Task<string> GetApiLocation(string apiNameWithoutEnv)
        {
            string qualifiedURI = null;

            try
            {
                var request = new LocGetApiLocation_Request
                {
                    ApiKey = ServiceContext.ApiKey,
                    AppName = apiNameWithoutEnv,
                    EnvInfo = new EnvInfo
                    {
                        Env = ServiceContext.SdkEnv,
                        IP = NetworkUtils.IP
                    }
                };

                var response = await SingleLocRequestPath.PostWebApi<string>(
                    m_Serializer.Serialize(request),
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
                );

                if (response.IsSuccess)
                {
                    qualifiedURI = response.Data;
                }
                else
                {
                    Logger.LogWarning($"LocationHelper: Failed to match service uri for {apiNameWithoutEnv}, msg: {response.ErrMsg} ");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return qualifiedURI;
        }

        public void Refresh()
        {
            if (m_ApiUriDic.IsEmpty)
            {
                return;
            }

            var appNameList = m_ApiUriDic.Keys.ToList();
            IDictionary<string, string> qualifiedURIs = null;

            try
            {
                var request = new LocBatchGetApiLocations_Request
                {
                    ApiKey = ServiceContext.ApiKey,
                    AppNames = appNameList,
                    EnvInfo = new EnvInfo
                    {
                        IP = NetworkUtils.IP,
                        Env = ServiceContext.SdkEnv
                    }
                };

                var response = BatchLocRequestPath.PostWebApi<IDictionary<string, string>>(
                    m_Serializer.Serialize(request),
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
                ).ConfigureAwait(false).GetAwaiter().GetResult();
                if (response.IsSuccess)
                {
                    qualifiedURIs = response.Data;
                    foreach (var key in qualifiedURIs.Keys)
                    {
                        m_ApiUriDic[key] = qualifiedURIs[key];
                    }
                }
                else
                {
                    Logger.LogError($"LocationHelper: Failed to Refresh error Message: {response.ErrMsg} ");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        private void CreateBackgroudThreadToSyncLocation()
        {
            m_Worker = new Thread(SyncCache)
            {
                IsBackground = true
            };
            m_Worker.Start();
        }

        private void SyncCache()
        {
            while (true)
            {
                if (IsStopBackgroudThread)
                {
                    break;
                }

                Refresh();
                Thread.Sleep(SyncInterval);
            }
        }

        public IDictionary<string, List<string>> GetAllLocationConfig()
        {
            return XMLReader.GetAllLocationConfig();
        }

        public IDictionary<string, ServiceEnvironment> RefreshEnvs()
        {
            IDictionary<string, ServiceEnvironment> result = null;

            try
            {
                lock (m_Lock)
                {
                    result = XMLReader.RefreshEnvs();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return result;
        }

        public void RefreshAppEnvIpMapping()
        {
            LocationHost.Instance.LoadDataFromLastSyncTime();
        }

        public void Dispose() { }

        public const int SyncInterval = 1 * 60 * 1000;
        public string SingleLocRequestPath { get; set; }
        public string BatchLocRequestPath { get; set; }
        public bool IsStopBackgroudThread { get; set; } = false;

        // (appName, url)
        protected readonly ConcurrentDictionary<string, string> m_ApiUriDic = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        protected readonly Lazy<LocationClient> m_Lazy = new Lazy<LocationClient>(() => new LocationClient());
        protected readonly object m_Lock = new object();
        protected readonly ISerializer m_Serializer;

        private Thread m_Worker;
    }
}
