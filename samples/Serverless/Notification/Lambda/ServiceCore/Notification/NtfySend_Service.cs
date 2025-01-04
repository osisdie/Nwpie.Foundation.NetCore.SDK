﻿using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Notification.Contract.Notification;
using Nwpie.Foundation.Notification.Lambda.Service.Common;
using Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification.Interfaces;
using Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification.Models;

namespace Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification
{
    public class NtfySend_Service : CustomServiceEntryBase<
       NtfySend_Request,
       NtfySend_Response,
       NotifySend_RequestModel,
       ResultDtoBase,
       NtfySend_ParamModel,
       INtfySend_DomainService>
    {
    }
}
