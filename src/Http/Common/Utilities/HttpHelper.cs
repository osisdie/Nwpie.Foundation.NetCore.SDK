using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Http.Common.Utilities
{
    public static class HttpHelper
    {
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            m_Accessor = httpContextAccessor;
        }

        public static void Save(this HttpContext context, string key, object value)
        {
            if (null == context)
            {
                return;
            }

            HttpContext.Items[key] = value;
        }

        public static bool TryGet<T>(this HttpContext context, string key, out T outValue)
        {
            outValue = context.Get<T>(key);
            return null != outValue;
        }

        public static T Get<T>(this HttpContext context, string key)
        {
            if (null == HttpContext?.Items)
            {
                return default;
            }

            if (HttpContext.Items.TryGetValue(key, out var s) && s is T value)
            {
                return value;
            }

            return default;
        }

        public static HttpContext HttpContext => m_Accessor?.HttpContext;

        private static IHttpContextAccessor m_Accessor;
    }
}
