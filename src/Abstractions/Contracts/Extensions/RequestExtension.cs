using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.ServiceNode;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Abstractions.Contracts.Extensions
{
    public static class RequestExtension
    {
        public static bool IsHealthCheckRequest(this string rawUrl) =>
           true == rawUrl?.Contains($"/{SNConst.HealthCheckController}", StringComparison.OrdinalIgnoreCase);

        public static int? ToOffset(this IRequestDto request, int startIndex = 0)
        {
            // if index starts from 0
            if (0 == startIndex)
            {
                return request.PageIndex * request.PageSize;
            }
            else
            {
                return (request.PageIndex - 1) * request.PageSize;
            }
        }

        public static bool Validate(this IRequestDto request) =>
            request.Validate(out _);

        public static bool Validate(this IRequestDto request, out List<string> msgList)
        {
            var result = ValidateUtils.Validate(request);
            msgList = result?.Data;
            return true == result?.IsSuccess;
        }

        public static bool Any<T>(this IServiceResponse<T> response) =>
            true == response?.IsSuccess &&
            null != response.Data;

        public static bool FailOrEmpty<T>(this IServiceResponse<T> response) =>
            true != response?.IsSuccess ||
            null == response.Data;

        public static IServiceResponse<T> Content<T>(this IServiceResponse<T> o, T data)
        {
            o.Data = data;
            return o;
        }

        public static IServiceResponse Error(this IServiceResponse o)
        {
            o.Error(StatusCodeEnum.Error, StatusCodeEnum.Error.ToString());
            return o;
        }

        public static IServiceResponse Error(this IServiceResponse o, StatusCodeEnum code, Exception ex)
        {
            o.Error(code, SdkRuntime.IsDebugOrDevelopment()
                ? ex?.GetBaseFirstExceptionString()
                : ex?.GetBaseFirstExceptionMessage());
            return o;
        }

        public static IServiceResponse Error(this IServiceResponse o, StatusCodeEnum code, string errMsg)
        {
            o.Code = (int)code;
            o.Msg = StatusCodeEnum.Error.ToString();
            o.ErrMsg = string.IsNullOrWhiteSpace(errMsg)
                ? code.ToString()
                : errMsg;
            o.IsSuccess = false;

            return o;
        }

        public static IServiceResponse<T> Error<T>(this IServiceResponse<T> o)
        {
            o.Error(StatusCodeEnum.Error, StatusCodeEnum.Error.ToString());
            return o;
        }

        public static IServiceResponse<T> Error<T>(this IServiceResponse<T> o, StatusCodeEnum code, Exception ex)
        {
            o.Error(code, SdkRuntime.IsDebugOrDevelopment()
                ? ex?.GetBaseFirstExceptionString()
                : ex?.GetBaseFirstExceptionMessage());
            return o;
        }

        public static IServiceResponse<T> Error<T>(this IServiceResponse<T> o, StatusCodeEnum code, string errMsg)
        {
            o.Code = (int)code;
            o.Msg = StatusCodeEnum.Error.ToString();
            o.ErrMsg = string.IsNullOrWhiteSpace(errMsg)
                ? code.ToString()
                : errMsg;
            o.IsSuccess = false;

            return o;
        }

        public static IServiceResponse Success(this IServiceResponse o)
        {
            o.Success(StatusCodeEnum.Success, StatusCodeEnum.Success.ToString());
            return o;
        }

        public static IServiceResponse Success(this IServiceResponse o, StatusCodeEnum code, string msg = null)
        {
            o.Code = (int)code;
            o.Msg = string.IsNullOrWhiteSpace(msg)
                ? code.ToString()
                : msg;
            o.IsSuccess = true;

            return o;
        }

        public static IServiceResponse<T> Success<T>(this IServiceResponse<T> o)
        {
            o.Success(StatusCodeEnum.Success, StatusCodeEnum.Success.ToString());
            return o;
        }

        public static IServiceResponse<T> Success<T>(this IServiceResponse<T> o, StatusCodeEnum code, string msg = null)
        {
            o.Code = (int)code;
            o.Msg = string.IsNullOrWhiteSpace(msg)
                ? code.ToString()
                : msg;
            o.IsSuccess = true;

            return o;
        }
    }
}
