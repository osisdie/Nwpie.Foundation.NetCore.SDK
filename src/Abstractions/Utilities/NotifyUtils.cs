using Nwpie.Foundation.Abstractions.Statics;

namespace Nwpie.Foundation.Common.Utilities
{
    public static class NotifyUtils
    {
        public static string SetTitlePrefixInDevelopment(this string title)
        {
            if (SdkRuntime.IsDebugOrDevelopment())
            {
                return $"[{SdkRuntime.SdkEnv}] {title ?? string.Empty}";
            }

            return title;
        }
    }
}
