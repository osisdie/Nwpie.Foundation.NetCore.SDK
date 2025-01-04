using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Notification.Lambda.Service.Common;
using Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification.Interfaces;
using Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification.Models;

namespace Nwpie.Foundation.Notification.Lambda.Service.ServiceCore.Notification.Services
{
    public class NtfySend_DomainService :
        DomainService,
        INtfySend_DomainService
    {
        public async Task<ResultDtoBase> Execute(NtfySend_ParamModel param)
        {
            Validate(param);

            var result = new ResultDtoBase();
            var service = new NotificationServiceCore(param);
            var apikeyProfile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(CurrentRequest,
                AuthExactFlagEnum.ApiKeyHeader |
                AuthExactFlagEnum.ApiKeyInToken
            );
            if (null == apikeyProfile)
            {
                throw new ArgumentNullException(StatusCodeEnum.InvalidApiKeyCredential.ToString());
            }

            var sendResult = await service.SendAsync();
            if (true == sendResult?.IsSuccess)
            {
                SubCode = sendResult.Code.ToString();
                SubMsg = sendResult.Msg;
                return result;
            }

            return null;
        }

        public bool Validate(NtfySend_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
