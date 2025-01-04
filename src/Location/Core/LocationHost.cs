using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Patterns;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Location.Core
{
    public class LocationHost : SingleCObject<LocationHost>, IRefreshable
    {
        protected override void InitialInConstructor()
        {
            Task.Run(() => WarmUp()).ConfigureAwait(false);
        }

        public void Start()
        {
            CreateBackgroudThreadToSyncApiKeyLocation();
        }

        public ServiceResponse<string> GetApiLocation(LocGetApiLocation_Request request)
        {
            var response = new ServiceResponse<string>();
            if (ValidateApikeyIP(request.ApiKey, request.EnvInfo?.IP, out var envCode, out var message, request.EnvInfo.Env))
            {
                response.Success();
                response.Data = XMLReader
                    .GetApiUris(envCode,
                        AuthServiceConfig.MatchApiNamePattern.Replace(request.AppName, "")
                    )?.FirstOrDefault();
            }
            else
            {
                response.Error(StatusCodeEnum.PermissionDeny, message);
            }

            return response;
        }

        public ServiceResponse<IDictionary<string, string>> BatchGetApiLocations(LocBatchGetApiLocations_Request request)
        {
            var response = new ServiceResponse<IDictionary<string, string>>();
            var message = string.Empty;
            var envCode = string.Empty;
            request.AppNames = request.AppNames
                ?? new List<string>();

            if (ValidateApikeyIP(request.ApiKey, request.EnvInfo.IP, out envCode, out message, request.EnvInfo.Env))
            {
                response.Success().Content(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                foreach (var item in request.AppNames)
                {
                    var url = XMLReader.GetApiUris(envCode, item).FirstOrDefault();
                    response.Data.Add(item, url);
                }
            }
            else
            {
                response.Error(StatusCodeEnum.PermissionDeny, message);
            }

            return response;
        }

        public bool ValidateApikeyIP(string apiKey, string ip, out string envCode, out string message, string clientEnv)
        {
            message = string.Empty;
            envCode = string.Empty;
            if (true != ServiceContext.Config.LOC?.ValidateIp)
            {
                envCode = clientEnv;
                return true;
            }

            var ipList = new List<string>();
            if (ip.HasValue())
            {
                ipList = ip.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (m_ApiKeyLocationInfoCache.TryGetValue(apiKey, out var apiKeyLocationInfo))
            {
                if (null != apiKeyLocationInfo)
                {
                    if (true == apiKeyLocationInfo.IPs
                        ?.Any(x => true == ipList?.Contains(x, StringComparer.OrdinalIgnoreCase)))
                    {
                        envCode = apiKeyLocationInfo.EnvCode;
                        return true;
                    }
                    else
                    {
                        // This IP didn't config for this apikey
                        message = $"Apikey[{apiKey}] can't be used in this IP [{ip}]. If want to use this IP, please config the IP for Apikey. ";
                        Logger.LogInformation(message);
                        return false;
                    }
                }
                else
                {
                    // Cannot find IP for this apikey
                    message = $"Apikey[{apiKey}] can't be used in this IP [{ip}]. If want to use this IP, please config the IP for Apikey. ";
                    Logger.LogInformation(message);
                    return false;
                }
            }
            else
            {
                // Cannot find this apikey
                message = $"Apikey[{apiKey}] has not been configed, please config Apikey and IP [{ip}]. ";
                Logger.LogInformation(message);
                return false;
            }
        }

        public void Refresh()
        {
            try
            {
                lock (m_Lock)
                {
                    m_LastSyncTime = DateTime.UtcNow.AddMinutes(-1);
                    var locations = ApiKeyService.Instance
                        .GetApiKeyLocationInfo()
                        ?? new List<LocationInfo>();

                    m_ApiKeyLocationInfoCache.Clear();
                    foreach (var item in locations)
                    {
                        m_ApiKeyLocationInfoCache.Add(item.ApiKey, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }
        }

        private void CreateBackgroudThreadToSyncApiKeyLocation()
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
                LoadDataFromLastSyncTime();
                Thread.Sleep(SyncInterval);
            }
        }

        public void LoadDataFromLastSyncTime()
        {
            try
            {
                lock (m_Lock)
                {
                    var lastSynctime = m_LastSyncTime;
                    m_LastSyncTime = DateTime.UtcNow.AddMinutes(-1);

                    var locations = ApiKeyService.Instance
                        .GetApiKeyLocationInfo(lastSynctime);
                    if (locations?.Count() > 0)
                    {
                        foreach (var item in locations)
                        {
                            if (m_ApiKeyLocationInfoCache.ContainsKey(item.ApiKey))
                            {
                                m_ApiKeyLocationInfoCache[item.ApiKey] = item;
                            }
                            else
                            {
                                m_ApiKeyLocationInfoCache.Add(item.ApiKey, item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }
        }

        private void WarmUp()
        {
            Refresh();
        }

        public override void Dispose()
        {

        }

        private const int SyncInterval = 10 * 60 * 1000;

        protected Thread m_Worker;
        protected readonly object m_Lock = new object();
        protected readonly Dictionary<string, LocationInfo> m_ApiKeyLocationInfoCache = new Dictionary<string, LocationInfo>(StringComparer.OrdinalIgnoreCase);
        protected DateTime m_LastSyncTime = new DateTime(2020, 1, 1);
    }
}
