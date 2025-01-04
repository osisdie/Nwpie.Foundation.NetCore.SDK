using System;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Common.Notification.Extensions
{
    public static class NotifySendRequestModel_Extension
    {
        public static NotifySend_RequestModel PreProcess(this NotifySend_RequestModel data)
        {
            if (false == data.SendTime.HasValue)
            {
                data.SendTime = DateTime.UtcNow;
            }

            if (string.IsNullOrWhiteSpace(data.Id))
            {
                data.Id = IdentifierUtils.NewId();
            }

            if (string.IsNullOrWhiteSpace(data.ApiName))
            {
                data.ApiName = ServiceContext.ApiName;
            }

            if (string.IsNullOrWhiteSpace(data.ApiKey))
            {
                data.ApiKey = ServiceContext.ApiKey;
            }

            return data;
        }
    }
}
