using System;
using System.Net;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Auth.SDK.Extensions;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Nwpie.Foundation.S3Proxy.Endpoint.Handlers
{
    public static class GlobalEventHandler
    {
        static GlobalEventHandler()
        {
            Logger = LogMgr.CreateLogger(typeof(GlobalEventHandler));
            //m_Cache = ComponentMgr.Instance.GetDefaultLocalCache(isUseDI: false);
        }

        public static void AddGlobalPreRequestFilters(this AppHostBase host)
        {
            host.PreRequestFilters.Add(async (req, res) =>
            {
                // Check before proxy
                if (true == req?.RawUrl.IsHealthCheckRequest())
                {
                    return;
                }

                var currentRequest = req.OriginalRequest as HttpRequest;
                try
                {
                    var apiKey = TokenUtils.GetApiKeyFromHeaderOrQuery(currentRequest);
                    if (apiKey.HasValue())
                    {
                        var profile = await TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(apiKey)
                            ?? throw new Exception(StatusCodeEnum.InvalidApiKeyCredential.GetDisplayName());

                        return;
                    }

                    var tokenService = ComponentMgr.Instance.GetDefaultAesTokenService()
                        ?? throw new Exception("Token " + StatusCodeEnum.ServiceCurrentlyUnavailable.GetDisplayName());

                    var encrypted = TokenUtils.GetTokenFromHeaderOrQuery(currentRequest);
                    var isValid = await tokenService.VerifyToken<TokenDataModel>(encrypted,
                        TokenKindEnum.AccessToken
                    );

                    if (true != isValid?.IsSuccess)
                    {
                        throw new Exception(isValid?.ErrMsg ?? isValid?.Msg);
                    }
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
                if (true == req?.RawUrl.IsHealthCheckRequest())
                {
                    return;
                }

                var log = await ServiceNodeUtils.ConvertToRequestLog(req, res?.Dto);
                Logger.LogInformation(log);
            });
        }

        public static void AddServiceExceptionHandlers(this AppHostBase host)
        {
            //Handle Exceptions occurring in Services:
            host.ServiceExceptionHandlersAsync.Add(async (req, request, ex) =>
            {
                var response = new ServiceResponse<object>()
                {
                    IsSuccess = false,
                    Code = (int)StatusCodeEnum.Error,
                    ErrMsg = Utility.ErrMsgDependOnEnv(ex),
                };

                var log = await ServiceNodeUtils.ConvertToResponseLog(req, response, ex);
                Logger.LogError(log);

                req.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                req.Response.ContentType = "application/json";
                req.Response.AddCorsHeader();

                await req.Response.WriteAsync(response.ToJson())
                    .ContinueWith(t => req.Response.EndRequest(skipHeaders: false));

                return req.Response;
            });
        }

        public static void AddUncaughtExceptionHandlers(this AppHostBase host)
        {
            host.UncaughtExceptionHandlers.Add(async (req, res, operationName, ex) =>
            {
                var response = new ServiceResponse<object>()
                {
                    IsSuccess = false,
                    Code = (int)StatusCodeEnum.Error,
                    ErrMsg = Utility.ErrMsgDependOnEnv(ex)
                };

                var log = await ServiceNodeUtils.ConvertToResponseLog(req, response, ex);
                Logger.LogError(log);

                req.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                req.Response.ContentType = "application/json";
                req.Response.AddCorsHeader();

                // Warning !!
                // StatusCode cannot be set because the response has already started.
                // Headers are read - only, response has already started.

                await res.WriteAsync(response.ToJson())
                    .ContinueWith(t => res.EndRequest(skipHeaders: true));
            });
        }

        private static readonly ILogger Logger;
        //private const string __ = CommonConst.Separator;
        //static readonly ICache m_Cache;
    }
}
