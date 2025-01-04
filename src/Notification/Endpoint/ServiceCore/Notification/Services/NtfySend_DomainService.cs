using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.MessageQueue.SQS.Interfaces;
using Nwpie.Foundation.Notification.Endpoint.Common;
using Nwpie.Foundation.Notification.Endpoint.ServiceCore.Notification.Interfaces;
using Nwpie.Foundation.Notification.Endpoint.ServiceCore.Notification.Models;

namespace Nwpie.Foundation.Notification.Endpoint.ServiceCore.Notification.Services
{
    public class NtfySend_DomainService :
        DomainService,
        INtfySend_DomainService
    {
        public async Task<ResultDtoBase> Execute(NtfySend_ParamModel param)
        {
            Validate(param);

            var publishResult = await m_NotifyClient.PublishAsync(param);
            return new ResultDtoBase
            {
                Message = publishResult.MsgId
            };
        }

        public bool Validate(NtfySend_ParamModel param)
        {
            if (null == m_NotifyClient)
            {
                throw new ArgumentNullException(nameof(IAwsSQSClient));
            }

            return base.ValidateAndThrow(param);
        }
    }
}
