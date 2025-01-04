using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.Abstractions.Enums
{
    /// <summary>
    /// Platform Error Code
    /// </summary>
    public enum StatusCodeEnum
    {
        [Display(Name = "unset")]
        UnSet = -0001,
        [Display(Name = "error")]
        Error = 0000,
        [Display(Name = "success")]
        Success = 0001,
        [Display(Name = "exception")]
        Exception = 0003,

        [Display(Name = "invalid apikey credenial")]
        InvalidApiKeyCredential = 0004,
        [Display(Name = "missing apikey")]
        MissingApiKey = 0005,
        [Display(Name = "invalid apiname or format")]
        InvalidApiName = 0006,
        [Display(Name = "missing apiname")]
        MissingApiName = 0007,
        [Display(Name = "missing request $.data")]
        MissingContractRequestData = 0008,
        [Display(Name = "serialization failed")]
        SerializationError = 0009,
        [Display(Name = "invalid request or format")]
        InvalidContractRequest = 0010,
        [Display(Name = "request validation failed")]
        ContractValidationError = 0011,
        [Display(Name = "missing request header")]
        MissingContractRequestHeader = 0012,
        [Display(Name = "http action not allowed")]
        HttpActionNotAllowed = 0013,
        [Display(Name = "service currently unavailable")]
        ServiceCurrentlyUnavailable = 0015,
        [Display(Name = "permission denied")]
        PermissionDeny = 0017,
        [Display(Name = "missing method")]
        MissingMethod = 0019,
        [Display(Name = "invalid method")]
        InvalidMethod = 0021,
        [Display(Name = "invalid file")]
        InvalidFile = 0022,
        [Display(Name = "invalid format")]
        InvalidFormat = 0023,
        [Display(Name = "invalid stream")]
        InvalidStream = 0024,
        [Display(Name = "missing session")]
        MissingSession = 0025,
        [Display(Name = "invalid session")]
        InvalidSession = 0027,
        [Display(Name = "missing appkey")]
        MissingAppKey = 0029,
        [Display(Name = "invalid appkey")]
        InvalidAppKey = 0031,
        [Display(Name = "missing timestamp")]
        MissingTimestamp = 0033,
        [Display(Name = "invalid timestamp or format")]
        InvalidTimestamp = 0035,
        [Display(Name = "missing version")]
        MissingVersion = 0037,
        [Display(Name = "invalid version")]
        InvalidVersion = 0039,
        [Display(Name = "missing access token")]
        MissingAccessToken = 0040,
        [Display(Name = "invalid access token or format")]
        InvalidAccessToken = 0041,
        [Display(Name = "invalid refresh token or format")]
        InvalidRefreshToken = 0042,
        [Display(Name = "access token expired")]
        AccessTokenExpired = 0043,
        [Display(Name = "refresh token expired")]
        RefreshTokenExpired = 0044,
        [Display(Name = "dismatch token source")]
        DismatchTokenSource = 0045,
        [Display(Name = "invalid parameter or format")]
        InvalidParameter = 0051,
        [Display(Name = "invalid encoding")]
        InvalidEncoding = 0052,
        [Display(Name = "empty data")]
        EmptyData = 0107,
        [Display(Name = "duplicate data")]
        DuplicateData = 0108,
        [Display(Name = "resource not found")]
        ResourceNotFound = 0200,
        [Display(Name = "null response")]
        NullResponse = 0300,

        ErrorCode_Max = 1000
    }
}
