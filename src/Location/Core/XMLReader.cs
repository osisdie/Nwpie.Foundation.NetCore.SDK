using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Location;

namespace Nwpie.Foundation.Location.Core
{
    public static class XMLReader
    {
        static XMLReader()
        {
            m_Cache = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
        }

        public static List<string> GetApiUris(string envName, string apiNameWithoutEnv)
        {
            var key = string.Format(FormatStyle, envName, apiNameWithoutEnv).ToLower();
            var result = new List<string>();
            if (true == m_ApiUriDic?.TryGetValue(key, out result))
            {
                return result;
            }

            return result;
        }

        private static IDictionary<string, List<string>> m_ApiUriDic
        {
            get
            {
                var cache = m_Cache.GetAsync<Dictionary<string, List<string>>>(ApiUrisCacheKey).ConfigureAwait(false).GetAwaiter().GetResult();
                if (cache.Any())
                {
                    return cache.Data as IDictionary<string, List<string>>;
                }

                return Refresh();
            }
        }

        private static IDictionary<string, ServiceEnvironment> m_EnvironmentDic
        {
            get
            {
                var cache = m_Cache.GetAsync<Dictionary<string, ServiceEnvironment>>(EnvironmentsCachekey).ConfigureAwait(false).GetAwaiter().GetResult();
                if (cache.Any())
                {
                    return cache.Data as IDictionary<string, ServiceEnvironment>;
                }

                return RefreshEnvs();
            }
        }

        public static IDictionary<string, List<string>> Refresh()
        {
            var apiUris = LoadApiUris();
            if (apiUris?.Count > 0 && null != m_Cache)
            {
                _ = m_Cache.SetAsync(ApiUrisCacheKey, apiUris).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return apiUris;
        }

        public static IDictionary<string, List<string>> GetAllLocationConfig()
        {
            return m_ApiUriDic;
        }

        public static IDictionary<string, ServiceEnvironment> RefreshEnvs()
        {
            var environments = LoadEnvironments();
            if (environments?.Count > 0 && null != m_Cache)
            {
                _ = m_Cache.SetAsync(EnvironmentsCachekey, environments).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return environments;
        }

        private static IDictionary<string, List<string>> LoadApiUris()
        {
            var environments = LoadEnvironments();
            var apiUris = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            if (environments != null)
            {
                foreach (var env in environments.Values)
                {
                    foreach (var doamin in env.Domains.Values)
                    {
                        foreach (var server in doamin.Servers.Values)
                        {
                            foreach (var app in server.Apps.Values)
                            {
                                var key = string.Format(FormatStyle, env.Name, app.Name).ToLower();
                                if (apiUris.Keys.Contains(key))
                                {
                                    apiUris[key].Add(app.Uri);
                                }
                                else
                                {
                                    apiUris.Add(key, new List<string>() { app.Uri });
                                }
                            }
                        }

                    }
                }
            }

            return apiUris;
        }

        private static IDictionary<string, ServiceEnvironment> LoadEnvironments()
        {
            var environments = new Dictionary<string, ServiceEnvironment>(StringComparer.OrdinalIgnoreCase);

            lock (m_Lock)
            {
                var xmlDoc = new XmlDocument();
                var xmlFilePath = LocatonUtils.SDK_ApiEnvironmentFilePath_DependsOnEnv();
                xmlDoc.Load(xmlFilePath);

                var rootNote = xmlDoc.SelectNodes("//Environment");
                foreach (XmlNode environmentNote in rootNote)
                {
                    if (environmentNote.NodeType == XmlNodeType.Element)
                    {
                        var envName = environmentNote.Attributes["Name"].Value.ToLower();
                        environments.Add(envName, new ServiceEnvironment(envName));

                        foreach (XmlNode domainNote in environmentNote.ChildNodes)
                        {
                            if (domainNote.NodeType == XmlNodeType.Element)
                            {
                                var domainName = domainNote.Attributes["Name"].Value.ToLower();
                                if (false == environments[envName].Domains.ContainsKey(domainName))
                                {
                                    environments[envName].Domains.Add(domainName, new Domain(domainName));
                                }

                                foreach (XmlNode serverNote in domainNote.ChildNodes)
                                {
                                    if (serverNote.NodeType == XmlNodeType.Element)
                                    {
                                        var serverIP = serverNote.Attributes["IP"].Value.ToLower();
                                        if (false == environments[envName].Domains[domainName].Servers.ContainsKey(serverIP))
                                        {
                                            environments[envName].Domains[domainName].Servers.Add(serverIP, new Server(serverIP));
                                        }

                                        foreach (XmlNode appNode in serverNote.ChildNodes)
                                        {
                                            if (appNode.NodeType == XmlNodeType.Element)
                                            {
                                                var app = new App
                                                {
                                                    Name = appNode.Attributes["Name"].Value.ToLower(),
                                                    Port = appNode.Attributes["Port"].Value.ToLower(),
                                                    Uri = appNode.Attributes["Uri"].Value.ToLower()
                                                };

                                                if (false == environments[envName].Domains[domainName].Servers[serverIP].Apps.ContainsKey(app.Name))
                                                {
                                                    environments[envName].Domains[domainName].Servers[serverIP].Apps.Add(app.Name, app);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return environments;
        }

        private const string ApiUrisCacheKey = "ApiUrisCacheKey";
        private const string EnvironmentsCachekey = "EnvironmentsCachekey";
        private const string FormatStyle = "{0}-{1}";

        private static readonly object m_Lock = new object();
        private static readonly ICache m_Cache;
    }
}
