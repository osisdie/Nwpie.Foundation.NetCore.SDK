using System;
using System.Collections.Generic;
using System.Net;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Enums;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Notification.Lambda.Service.Handlers
{
    public static class ExceptionMiddlewareExtensions
    {
        static ExceptionMiddlewareExtensions()
        {
            Logger = LogMgr.CreateLogger(typeof(ExceptionMiddlewareExtensions));
            m_Serializer = new DefaultSerializer();
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features
                        .Get<IExceptionHandlerFeature>();
                    if (null != contextFeature)
                    {
                        var requestDto = await context
                            .Request
                            .FormatRequest(context);

                        var responseDto = new ServiceResponse<string>()
                            .Error(StatusCodeEnum.Exception, Utility.ErrMsgDependOnEnv(contextFeature.Error));

                        Logger.LogError(m_Serializer.Serialize(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        {
                            { SysLoggerKey.Type, LoggingTypeEnum.Response.GetDisplayName() },
                            { SysLoggerKey.ResponseDto, responseDto },
                            { SysLoggerKey.RequestDto, requestDto },
                            { SysLoggerKey.Headers, context.Request.Headers },
                            { SysLoggerKey.Url, context.Request.Path.Value },
                            { SysLoggerKey.ClientIP, context.Connection.RemoteIpAddress },
                            { SysLoggerKey.Exception, contextFeature.Error },
                        }.AddTraceData()));

                        await context.Response
                            .WriteAsync(m_Serializer.Serialize(responseDto));
                    }
                });
            });
        }

        private static readonly ILogger Logger;
        private static readonly ISerializer m_Serializer;
    }
}
