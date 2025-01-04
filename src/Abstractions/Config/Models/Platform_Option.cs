using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Config.Interfaces;

namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class Platform_Option : IPlatform_Option
    {
        public string SdkEnv { get; set; }
        public string ApiName { get; set; }
        public string ApiKey { get; set; }
        public string BaseServiceUrl { get; set; }

        // (filePath, isOptional)
        public Dictionary<string, bool> StartupJsonFileList { get; set; }

        public List<string> RemoteConfigKeys { get; set; }
        public Dictionary<string, string> ConnectionStrings { get; set; }
        public Dictionary<string, bool> FeatureToggle { get; set; }
        public DateTime? _ts { get; set; }
    }
}
