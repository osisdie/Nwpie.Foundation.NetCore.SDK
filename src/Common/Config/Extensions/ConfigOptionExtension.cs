using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Common.Extras;

namespace Nwpie.Foundation.Common.Config.Extensions
{
    public static class ConfigOptionExtension
    {
        public static string ToString(this ConfigOptions o)
        {
            var serializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            if (null != serializer)
            {
                return serializer.Serialize(o);
            }

            return o.ToString();
        }

        public static string ToString<T>(this ConfigOptions<T> o)
            where T : class, new()
        {
            var serializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            if (null != serializer)
            {
                return serializer.Serialize(o);
            }

            return o.ToString();
        }
    }
}
