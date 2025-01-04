using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Config.Interfaces
{
    public interface IPlatform_Option : IOption
    {
        string SdkEnv { get; set; }
        string ApiName { get; set; }
        string ApiKey { get; set; }
        string BaseServiceUrl { get; set; }

        // (filepath, isOptional)
        Dictionary<string, bool> StartupJsonFileList { get; set; }
        List<string> RemoteConfigKeys { get; set; }
        Dictionary<string, string> ConnectionStrings { get; set; }
        Dictionary<string, bool> FeatureToggle { get; set; }
    }
}
