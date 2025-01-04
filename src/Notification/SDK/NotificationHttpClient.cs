using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Http.Common.Utilities;

namespace Nwpie.Foundation.Notification.SDK
{
    public class NotificationHttpClient : CObject, INotificationHttpClient
    {
        public NotificationHttpClient(ISerializer serializer) : base()
        {
            m_Serializer = serializer
                ?? throw new ArgumentException(nameof(ISerializer));
        }

        public NotificationHttpClient(string notificationHostUrl, ISerializer serializer) : base()
        {
            NotificationHostUrl = notificationHostUrl;
            m_Serializer = serializer
                ?? throw new ArgumentException(nameof(ISerializer));
        }

        public virtual async Task<NotifySend_Response> SendAsync(NotifySend_Request request)
        {
            await IsReady();

            if (null == request?.Data ||
                request.Data.Kind <= 0 ||
                string.IsNullOrWhiteSpace(request.Data.Title) ||
                string.IsNullOrWhiteSpace(request.Data.ToList))
            {
                throw new ArgumentNullException($@"Arguments
                    {nameof(request.Data.Title)},
                    {nameof(request.Data.Kind)},
                    {nameof(request.Data.Message)},
                    {nameof(request.Data.ToList)} are required. ");
            }

            // POST
            var jsonData = m_Serializer.Serialize(request);
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)//.AddApiKeyHeader()
            {
                { CommonConst.ApiName, ServiceContext.ApiName },
                { CommonConst.ApiKey, ServiceContext.ApiKey }
            };

            var result = new NotifySend_Response();

            try
            {
                await RetryHelper.RetryOnException(ConfigConst.DefaultHttpRetry,
                    TimeSpan.FromSeconds(ConfigConst.DefaultDelayRetrySecs),
                    async () =>
                    {
                        var response = await ApiUtils.HttpRequestAsync<NotifySend_Response>(HttpMethod.Post,
                            NotificationHostUrl,
                            jsonData,
                            headers,
                            m_Serializer,
                            DefaultTimeoutSecs
                        );

                        if (true != response.Any())
                        {
                            throw new Exception(response.ErrMsg);
                        }

                        result = response.Data;
                    }
                );
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, ex.GetBaseFirstExceptionString());
            }

            return result;
        }

        public virtual async Task<bool> IsReady()
        {
            var isReady = true;
            if (string.IsNullOrWhiteSpace(ServiceContext.ApiName))
            {
                throw new ArgumentNullException(nameof(ServiceContext.ApiName));
            }

            if (string.IsNullOrWhiteSpace(ServiceContext.ApiKey))
            {
                throw new ArgumentNullException(nameof(ServiceContext.ApiKey));
            }

            if (string.IsNullOrWhiteSpace(NotificationHostUrl))
            {
                throw new ArgumentNullException(nameof(NotificationHostUrl));
            }

            await Task.CompletedTask;
            return isReady;
        }

        public string NotificationHostUrl { get; set; }
        public int DefaultTimeoutSecs { get; set; } = ConfigConst.DefaultHttpTimeout;
        public int DefaultRetries { get; set; } = ConfigConst.DefaultHttpRetry;
        public int DefaultDelayRetrySecs { get; set; } = ConfigConst.DefaultDelayRetrySecs;

        protected readonly ISerializer m_Serializer;
    }
}
