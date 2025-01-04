using System;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class WebUtils
    {
        public static string GetPathWithoutQuery(this string pathAndQuery, string trimSvc = null)
        {
            var url = pathAndQuery.IndexOf('?') > 0
                ? pathAndQuery.Substring(0, pathAndQuery.IndexOf('?'))
                : pathAndQuery;

            return url.TrimStartServiceName(trimSvc);
        }

        public static string TrimStartServiceName(this string url, string trimSvc)
        {
            return string.IsNullOrEmpty(trimSvc)
                ? url
                : (url.IndexOf($"/{trimSvc}") >= 0
                    ? url.Substring(trimSvc.Length + 1)
                    : url);
        }

        public static string ExtractHostUrl(this string absoluteUrl, string svc = null)
        {
            try
            {
                var uri = new Uri(absoluteUrl);
                return uri.Host + (string.IsNullOrEmpty(svc)
                    ? ""
                    : $"/{svc}");
            }
            catch { return string.Empty; }
        }

        public static string GetHttpResponseContentType(string extension)
        {
            switch (extension)
            {
                case "jpg":
                case "jpeg":
                    return "image/jpeg";

                case "tif":
                case "tiff":
                    return "image/tiff";

                case "png":
                    return "image/png";

                default:
                    return "application/octet-stream";
            }
        }
    }
}
