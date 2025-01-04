using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Http.Common.Utilities;
using ServiceStack;

namespace Nwpie.Foundation.ServiceNode.SDK.Extensions
{
    public static class ServiceClientExtension
    {
        public static async Task<TResponse> InvokeAsyncByServiceName<TResponse>(this object request,
            string serviceName,
            string httpMethod = "POST",
            IDictionary<string, string> headers = null,
            int timeoutSecs = ConfigConst.DefaultHttpTimeout)
        {
            // Get path from LocationService
            var client = ComponentMgr.Instance.TryResolve<ILocationClient>();
            var baseUrl = string.Empty;
            if (null != client)
            {
                baseUrl = await client.GetApiLocation(serviceName);
            }

            if (baseUrl.HasValue())
            {
                var absoleteUrl = string.Concat(
                    baseUrl.TrimEndSlash(),
                    request.ToUrl(httpMethod)
                );

                return await request.InvokeAsyncByAbsoleteUrl<TResponse>(
                    url: absoleteUrl,
                    httpMethod: httpMethod,
                    headers: headers,
                    timeoutSecs: timeoutSecs
                );
            }

            throw new OperationCanceledException($"Failed to resolve uri of service(={serviceName}) ");
        }

        public static async Task<TResponse> InvokeAsyncByBaseUrl<TResponse>(this object request,
            string baseUrl,
            string httpMethod = "POST",
            IDictionary<string, string> headers = null,
            int timeoutSecs = ConfigConst.DefaultHttpTimeout)
        {
            var absoleteUrl = string.Concat(
                baseUrl.TrimEndSlash(),
                request.ToUrl(httpMethod)
            );

            return await request.InvokeAsyncByAbsoleteUrl<TResponse>(
                url: absoleteUrl,
                httpMethod: httpMethod,
                headers: headers,
                timeoutSecs: timeoutSecs
            );
        }

        public static async Task<TResponse> InvokeAsyncByAbsoleteUrl<TResponse>(this object request,
            string url,
            string httpMethod = "POST",
            IDictionary<string, string> headers = null,
            int timeoutSecs = ConfigConst.DefaultHttpTimeout)
        {
            IServiceResponse<TResponse> result;
            switch (httpMethod.ToUpper())
            {
                case "POST":
                    {
                        result = await ApiUtils.PostWebApi<TResponse>(url: url,
                            jsonData: request.ToJson(),
                            headers: headers,
                            timeoutSecs: timeoutSecs
                        );
                        break;
                    }

                case "GET":
                    {
                        result = await ApiUtils.GetWebApi<TResponse>(url: url,
                            jsonData: request.ToJson(),
                            headers: headers,
                            timeoutSecs: timeoutSecs
                        );
                        break;
                    }

                case "PUT":
                    {
                        result = await ApiUtils.PutWebApi<TResponse>(url: url,
                            jsonData: request.ToJson(),
                            headers: headers,
                            timeoutSecs: timeoutSecs
                        );
                        break;
                    }

                case "DELETE":
                    {
                        result = await ApiUtils.DeleteWebApi<TResponse>(url: url,
                            jsonData: request.ToJson(),
                            headers: headers,
                            timeoutSecs: timeoutSecs
                        );
                        break;
                    }

                default:
                    throw new InvalidOperationException($"Unknown http method={httpMethod}, ");
            }

            if (null == result)
            {
                return default(TResponse);
            }

            return result.Data;
        }
    }
}
