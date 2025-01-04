using System;
using System.Net;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.MiniSite.ES.Endpoint.Handlers
{
    public static class GlobalEventHandler
    {
        static GlobalEventHandler()
        {
            Logger = LogMgr.CreateLogger(typeof(GlobalEventHandler));
            //m_Cache = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
            m_TokenHander = new TokenHandler();
        }

        public static void AddGlobalPreRequestFilters(this AppHostBase host)
        {
            host.PreRequestFilters.Add(async (req, res) =>
            {
                if (true == req?.RawUrl.IsHealthCheckRequest())
                {
                    return;
                }

                IsFirstRequest();

                // Either valid token or apikey
                var currentRequest = req.OriginalRequest as HttpRequest;
                try
                {
                    var apiKey = TokenUtils.GetApiKeyFromHeaderOrQuery(currentRequest);
                    if (apiKey.HasValue())
                    {
                        var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey);
                        if (null == profile)
                        {
                            throw new Exception(StatusCodeEnum.InvalidApiKeyCredential.ToString());
                        }

                        return;
                    }

                    var isValidBearer = await m_TokenHander.IsValidToken(currentRequest);
                    if (false == isValidBearer)
                    {
                        throw new Exception(StatusCodeEnum.InvalidAccessToken.ToString());
                    }

                    // TODO: Clear Authorization content
                    //if (req.Headers[CommonConst.AuthHeaderName].HasValue())
                    //{
                    //    req.Headers.Remove(CommonConst.AuthHeaderName);
                    //}
                }
                catch (Exception ex)
                {
                    var responseDto = new ServiceResponse<object>(false)
                        .Error(StatusCodeEnum.InvalidAccessToken, ex.Message);

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
                }
            });
        }

        public static void AddGlobalRequestFilters(this AppHostBase host)
        {
            host.GlobalRequestFiltersAsync.Add(async (req, res, requestDto) =>
            {
                var log = await ServiceNodeUtils.ConvertToRequestLog(req, res?.Dto);
                Logger.LogInformation(log);
            });
        }

        public static void AddServiceExceptionHandlers(this AppHostBase host)
        {
            //Handle Exceptions occurring in Services:
            host.ServiceExceptionHandlersAsync.Add(async (req, requestDto, ex) =>
            {
                var responseDto = new ServiceResponse<object>(false)
                    .Error(StatusCodeEnum.Error, Utility.ErrMsgDependOnEnv(ex));

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

        static void IsFirstRequest()
        {
            if (false == IsFirstRequested)
            {
                lock (m_Lock)
                {
                    if (false == IsFirstRequested)
                    {
                        IsFirstRequested = true;
                        m_TokenHander.Start();
                    }
                }
            }
        }

        private static readonly ILogger Logger;
        //private static readonly ICache m_Cache;
        private static readonly object m_Lock = new();
        private static readonly TokenHandler m_TokenHander;

        private static bool IsFirstRequested;
    }
}
