using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.ApiKey.Models
{
    [Serializable]
    public class ServiceEnvironment
    {
        public ServiceEnvironment(string name)
        {
            Name = name;
            Domains = new Dictionary<string, Domain>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; set; }
        public IDictionary<string, Domain> Domains { get; set; }
    }

    [Serializable]
    public class Domain
    {
        public Domain(string name)
        {
            Name = name;
            Servers = new Dictionary<string, Server>(StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; set; }
        public IDictionary<string, Server> Servers { get; set; }
    }

    [Serializable]
    public class Server
    {
        public Server(string ip)
        {
            IP = ip;
            Apps = new Dictionary<string, App>(StringComparer.OrdinalIgnoreCase);
        }

        public string IP { get; set; }
        public IDictionary<string, App> Apps { get; set; }
    }

    [Serializable]
    public class App
    {
        public string Name { get; set; }
        public string Port { get; set; }
        public string Uri { get; set; }
        public string HealthCheck { get; set; }
    }
}
