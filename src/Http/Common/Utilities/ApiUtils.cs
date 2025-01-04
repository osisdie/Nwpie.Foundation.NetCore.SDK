using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Logging.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Http.Common.Utilities
{
    public static class ApiUtils
    {
        static ApiUtils()
        {
            Logger = LogMgr.CreateLogger(typeof(ApiUtils));
        }

        public static async Task<IServiceResponse<string>> HttpPost(string url, string jsonData, IDictionary<string, string> headers = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<string>(HttpMethod.Post,
                url,
                jsonData,
                headers,
                null,
                timeoutSecs
            );

        public static async Task<IServiceResponse<string>> HttpGet(string url, IDictionary<string, string> headers = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<string>(HttpMethod.Get,
                url,
                null,
                headers,
                null,
                timeoutSecs
            );

        public static async Task<IServiceResponse<string>> HttpPut(string url, string jsonData, IDictionary<string, string> headers = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<string>(HttpMethod.Put,
                url,
                jsonData,
                headers,
                null,
                timeoutSecs
            );

        public static async Task<IServiceResponse<string>> HttpDelete(string url, string jsonData, IDictionary<string, string> headers = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<string>(HttpMethod.Delete,
                url,
                jsonData,
                headers,
                null,
                timeoutSecs
            );

        public static async Task<IServiceResponse<T>> GetWebApi<T>(this string url, string jsonData, IDictionary<string, string> headers = null, ISerializer serializer = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<T>(HttpMethod.Get,
                url,
                jsonData,
                headers,
                serializer,
                timeoutSecs
            );

        public static async Task<IServiceResponse<T>> PostWebApi<T>(this string url, string jsonData, IDictionary<string, string> headers = null, ISerializer serializer = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<T>(HttpMethod.Post,
                url,
                jsonData,
                headers,
                serializer,
                timeoutSecs
            );

        public static async Task<IServiceResponse<T>> PutWebApi<T>(this string url, string jsonData, IDictionary<string, string> headers = null, ISerializer serializer = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<T>(HttpMethod.Put,
                url,
                jsonData,
                headers,
                serializer,
                timeoutSecs
            );

        public static async Task<IServiceResponse<T>> DeleteWebApi<T>(this string url, string jsonData, IDictionary<string, string> headers = null, ISerializer serializer = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout) =>
            await HttpRequestAsync<T>(HttpMethod.Delete,
                url,
                jsonData,
                headers,
                serializer,
                timeoutSecs
            );

        /// <summary>
        /// Http method with contet-type = application/json
        /// Get strong type response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="jsonData"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<IServiceResponse<T>> HttpRequestAsync<T>(HttpMethod method, string url, string jsonData, IDictionary<string, string> headers = null, ISerializer serializer = null, int timeoutSecs = ConfigConst.DefaultHttpTimeout)
        {
            var result = new ServiceResponse<T>(true);
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            request.KeepAlive = false;
            request.Timeout = timeoutSecs * 1000;
            request.ContentType = "application/json";
            request.Method = method.Method;

            byte[] jsonDataBytes = null;
            if (null != jsonData)
            {
                jsonDataBytes = Encoding.UTF8.GetBytes(jsonData);
                request.ContentLength = jsonDataBytes.Length;
            }

            if (null != headers)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            WebResponse response;
            try
            {
                if (null != jsonDataBytes)
                {
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                    }
                }

                response = await request.GetResponseAsync();
                if (response is HttpWebResponse res)
                {
                    result.IsSuccess =
                        HttpStatusCode.OK == res.StatusCode ||
                        HttpStatusCode.NoContent == res.StatusCode;
                }
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Error, ex.ToString());
                result.Msg = "WebRequest failed. ";
                Logger.LogWarning(Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { SysLoggerKey.Type, LoggingTypeEnum.RequestException.GetDisplayName() },
                    { SysLoggerKey.Requester, ServiceContext.ApiName },
                    { SysLoggerKey.RequestDto, jsonData },
                    { SysLoggerKey.Headers, headers },
                    { SysLoggerKey.Url, url },
                    { SysLoggerKey.ClientIP, NetworkUtils.IP },
                    { SysLoggerKey.Exception, ex },
                }.AddTraceData()));

                return result;
            }

            string responseBody = null;
            try
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var stream = new StreamReader(responseStream))
                    {
                        responseBody = await stream.ReadToEndAsync();
                        if (false == result.IsSuccess)
                        {
                            result.ErrMsg = responseBody;
                        }
                        else
                        {
                            if (typeof(T) == typeof(string))
                            {
                                result.Data = (T)Convert.ChangeType(responseBody, typeof(string));
                            }
                            else
                            {
                                result.Data = (serializer ?? Serializer).Deserialize<T>(responseBody);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.SerializationError, ex.ToString());
                result.Msg = StatusCodeEnum.SerializationError.ToString();
                Logger.LogWarning(Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { SysLoggerKey.Type, LoggingTypeEnum.ResponseException.GetDisplayName() },
                    { SysLoggerKey.Requester, ServiceContext.ApiName },
                    { SysLoggerKey.ResponseDto, responseBody },
                    { SysLoggerKey.RequestDto, jsonData },
                    { SysLoggerKey.Headers, headers },
                    { SysLoggerKey.Url, url },
                    { SysLoggerKey.ClientIP, NetworkUtils.IP },
                    { SysLoggerKey.Exception, ex },
                }.AddTraceData()));
            }

            return result;
        }

        public static async Task<HttpResponseLog> FormatResponse(HttpResponse response)
        {
            var log = new HttpResponseLog()
            {
                StatusCode = response.StatusCode,
                Headers = FormatHeader(response.Headers)
            };

            using (var streamReader = new StreamReader(response.Body))
            {
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                log.Body = await streamReader.ReadToEndAsync();
            }

            // Reset
            response.Body.Seek(0, SeekOrigin.Begin);
            return log;
        }

        public static IEnumerable<KeyValuePair<string, string>> FormatHeader(IHeaderDictionary header) =>
            header?.Where(o => false == string.IsNullOrWhiteSpace(o.Value))
                ?.ToDictionary(
                    item => item.Key,
                    item => item.ToString()
                );

        private static readonly ILogger Logger;
        private static readonly ISerializer Serializer = new DefaultSerializer();
    }
}
