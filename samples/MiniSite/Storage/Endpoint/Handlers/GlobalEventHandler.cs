using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Logging.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Utilities;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.Endpoint.Handlers
{
    public static class GlobalEventHandler
    {
        static GlobalEventHandler()
        {
            Logger = LogMgr.CreateLogger(typeof(GlobalEventHandler));
            //m_Cache = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
        }

        public static void AddGlobalRequestFilters(this AppHostBase host)
        {
            host.GlobalRequestFiltersAsync.Add(async (req, res, requestDto) =>
            {
                if (true == req.RawUrl.IsHealthCheckRequest())
                {
                    return;
                }

                var log = await ServiceNodeUtils.ConvertToRequestLog(req, res?.Dto);
                Logger.LogInformation(log);
            });
        }

        public static void AddGlobalResponseFilters(this AppHostBase host)
        {
            host.GlobalResponseFiltersAsync.Add(async (req, res, responseDto) =>
            {
                if (true == req.RawUrl.IsHealthCheckRequest())
                {
                    return;
                }

                // Inject Response
                // Only if isSuccess is true
                var isSuccess = responseDto?.GetType()
                    ?.GetProperty(nameof(ContractResponseBase.IsSuccess))
                    ?.GetValue(responseDto, null);
                if (null == isSuccess || false == (bool)isSuccess)
                {
                    return;
                }

                var data = responseDto?.GetType()
                    ?.GetProperty(nameof(ContractResponseBase.ExtendedDictionary))
                    ?.GetValue(responseDto, null);
                if (!(data is IDictionary<string, string> map))
                {
                    return;
                }

                if (ServiceContext.IsDebugOrDevelopment())
                {
                    map.AddDebugData();
                }

                await Task.CompletedTask;
            });
        }

        public static void AddServiceExceptionHandlers(this AppHostBase host)
        {
            //Handle Exceptions occurring in Services:
            host.ServiceExceptionHandlersAsync.Add(async (req, requestDto, ex) =>
            {
                var responseDto = new ServiceResponse<object>()
                {
                    IsSuccess = false,
                    Code = (int)StatusCodeEnum.Error,
                    ErrMsg = Utility.ErrMsgDependOnEnv(ex),
                };

                var log = await ServiceNodeUtils.ConvertToResponseLog(req, responseDto, ex);
                Logger.LogError(log);

                req.Response.StatusCode = (int)HttpStatusCode.OK;
                req.Response.ContentType = "application/json";
                req.Response.AddCorsHeader();

                await req.Response.WriteAsync(responseDto.ToJson())
                    .ContinueWith(t => req.Response.EndRequest(skipHeaders: false));

                return req.Response;
            });
        }

        public static void AddUncaughtExceptionHandlers(this AppHostBase host)
        {
            host.UncaughtExceptionHandlersAsync.Add(async (req, res, operationName, ex) =>
            {
                var responseDto = new ServiceResponse<object>(false)
                    .Error(StatusCodeEnum.Error, Utility.ErrMsgDependOnEnv(ex));

                var log = await ServiceNodeUtils.ConvertToResponseLog(req, responseDto, ex);
                Logger.LogError(log);

                req.Response.StatusCode = (int)HttpStatusCode.OK;
                req.Response.ContentType = "application/json";
                req.Response.AddCorsHeader();

                // Warning !!
                // StatusCode cannot be set because the response has already started.
                // Headers are read - only, response has already started.

                await res.WriteAsync(responseDto.ToJson())
                    .ContinueWith(t => res.EndRequest(skipHeaders: true));
            });
        }

        private static readonly ILogger Logger;
        //private static readonly ICache m_Cache;
        //static readonly int CacheDurationSecs = 5 * 60;
    }
}
