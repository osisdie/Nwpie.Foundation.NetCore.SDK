using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.ServiceNode.ServiceStack.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Nwpie.MiniSite.KVS.Endpoint.Handlers
{
    public class ModelStateValidator : IAsyncActionFilter
    {
        static ModelStateValidator()
        {
            Logger = LogMgr.CreateLogger(typeof(ModelStateValidator));
            m_Serializer = new DefaultSerializer();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (false == context.ModelState.IsValid)
            {
                var errors = string.Join(" ", context.ModelState
                    .SelectMany(v => v.Value.Errors)
                    .Select(x => x.ErrorMessage));

                var responseDto = new ServiceResponse<string>(false)
                    .Error(StatusCodeEnum.InvalidContractRequest, errors);

                var requestDto = await context
                    .HttpContext
                    .Request
                    .FormatRequest(context.HttpContext);

                var tokenDetail = await TokenUtils.GetTokenDetail(context.HttpContext.Request);
                Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { SysLoggerKey.Type, LoggingTypeEnum.Response.GetDisplayName() },
                    { SysLoggerKey.Contract, context.ActionDescriptor?.DisplayName },
                    { SysLoggerKey.Requester, tokenDetail?.GetRequester() },
                    { SysLoggerKey.AccountId, tokenDetail?.GetAccountId() },
                    { SysLoggerKey.AccountLevel, tokenDetail?.GetAccountLevel() },
                    { SysLoggerKey.ResponseDto, responseDto },
                    { SysLoggerKey.RequestDto, requestDto },
                    { SysLoggerKey.Headers, context.HttpContext.Request.Headers },
                    { SysLoggerKey.Url, context.HttpContext.Request.Path.Value },
                    { SysLoggerKey.ClientIP, context.HttpContext.Connection.RemoteIpAddress },
                    { SysLoggerKey.Exception, errors },
                }.AddTraceData()));

                context.Result = new JsonResult(responseDto);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context) { }

        private static readonly ILogger Logger;
        private static readonly ISerializer m_Serializer;
    }
}
