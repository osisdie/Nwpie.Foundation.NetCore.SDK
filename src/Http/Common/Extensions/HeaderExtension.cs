using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Http.Common.Extensions
{
    public static class HeaderExtension
    {
        public static NameValueCollection ToNameValueCollection(this IHeaderDictionary headers)
        {
            var collection = new NameValueCollection();
            if (null != headers)
            {
                foreach (var key in headers.Keys)
                {
                    collection.Add(key, headers[key]);
                }
            }

            return collection;
        }
    }
}
