using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers;

namespace Nwpie.Foundation.Caching.Common.Extensions
{
    public static class ICacheExtension
    {
        static ICacheExtension()
        {
            Serializer = new DefaultSerializer();
        }

        public static bool IsSlowResponse(this IServiceResponse o)
        {
            return o.IsSlowResponse(out _);
        }

        public static bool IsSlowResponse(this IServiceResponse o, out double ms)
        {
            ms = 0;
            if (true == o?.SubMsg?.HasValue())
            {
                var dict = Serializer.Deserialize<Dictionary<string, string>>(o.SubMsg, ignoreException: true);
                var duration = string.Empty;
                if (true == dict?.TryGetValue(SysLoggerKey.MillisecondsDuration, out duration) &&
                    true == double.TryParse(duration, out ms))
                {
                    return ms >= MinimumMillisecondsDuration;
                }
            }

            return false;
        }

        public const int DefaultMinimumMillisecondsDuration = 199;
        public static int MinimumMillisecondsDuration = DefaultMinimumMillisecondsDuration;

        private static readonly ISerializer Serializer;
    }
}
