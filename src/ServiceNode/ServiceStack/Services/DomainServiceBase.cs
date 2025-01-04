using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Extensions;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Http.Common.Extensions;
using Nwpie.Foundation.Http.Common.Interfaces;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Services
{
    public abstract class DomainServiceBase : CObject, IDomainService
    {
        public DomainServiceBase()
        {
            if (null != HttpHelper.HttpContext?.Items)
            {
                if (HttpHelper.HttpContext.Items.TryGetValue(nameof(IRequestService), out var s) &&
                    s is IRequestService svc)
                {
                    ServiceEntry = svc;
                }
            }

            Headers = CurrentRequest?.Headers?.ToNameValueCollection()
                ?? new NameValueCollection();

            ClearResult();
            Initialization();
        }

        protected virtual void Initialization() { }

        public void ClearResult()
        {
            SubCode = string.Empty;
            SubMsg = string.Empty;
            IsCacheHit = false;
            TotalCount = 0;
            ReturnCount = 0;
            Requester = null;
            //Headers.Clear();
        }

        public virtual void FillCacheInfo<T>(T responseDto, string cacheKey)
            where T : ResultDtoBase
        {
            if (string.IsNullOrWhiteSpace(cacheKey) || null == GetCache())
            {
                return;
            }

            responseDto.IsFromCache = true;
            responseDto.CacheKey = cacheKey;
            responseDto.CacheProviderName = GetCache()?.GetType()?.Name;
            responseDto.CacheTime = DateTime.UtcNow;

            if (ServiceContext.IsProduction() && responseDto.CacheKey.HasValue())
            {
                responseDto.CacheKey = responseDto.CacheKey
                    .Substring(responseDto.CacheKey.Length - responseDto.CacheKey.Length / 2)
                    .PadLeft(responseDto.CacheKey.Length, '*');
            }
        }

        public virtual void WrapBadResponseAndThrow(IServiceResponse responseDto)
        {
            SubCode = responseDto?.SubCode.AssignIf(o => false == o.HasValue(), responseDto?.Code.ToString());
            SubMsg = responseDto?.ErrMsg.AssignIf(o => false == o.HasValue(), responseDto?.Msg);
            if (false == responseDto?.IsSuccess)
            {
                throw new Exception(SubMsg);
            }
        }

        public virtual bool Validate(object param)
        {
            IsValidParam = ValidateAndFillMessage(param);
            return IsValidParam;
        }

        public virtual bool ValidateAndThrow(object param)
        {
            IsValidParam = Validate(param);
            if (false == IsValidParam)
            {
                throw new ArgumentException(SubMsg ?? StatusCodeEnum.InvalidContractRequest.ToString());
            }

            return IsValidParam;
        }

        protected bool ValidateAndFillMessage(object param)
        {
            var isValid = ValidateUtils.Validate(param);
            if (true != isValid?.IsSuccess)
            {
                SubCode = StatusCodeEnum.InvalidContractRequest.ToString();
            }

            if (isValid.Data?.Count > 0)
            {
                SubMsg = string.Join(" ", isValid.Data);
            }

            return true == isValid?.IsSuccess;
        }

        public virtual void ValidatePagingNumber(RequestDtoBase param)
        {
            // param.PageIndex starts with 0
            // param.PageSize at least = 1
            if (false == param.PageIndex.HasValue ||
                param.PageIndex < ConfigConst.MinPageIndex ||
                false == param.PageSize.HasValue ||
                param.PageSize <= 0)
            {
                throw new ArgumentException("Wrong paging number. ");
            }

            if (param.PageSize < ConfigConst.MinPageSize)
            {
                param.PageSize = ConfigConst.DefaultPageSize;
            }

            if (param.PageSize > ConfigConst.MaxPageSize)
            {
                param.PageSize = ConfigConst.MaxPageSize;
            }
        }

        public virtual T_ToDto ConvertTo<T_FromDto, T_ToDto>(T_FromDto src)
        {
            if (null == src)
            {
                return default(T_ToDto);
            }

            return ComponentMgr.Instance.Resolve<IMapperMgr>()
                .ConvertTo<T_FromDto, T_ToDto>(src);
        }

        public virtual IEnumerable<T_ToDto> ConvertAll<T_FromDto, T_ToDto>(IEnumerable<T_FromDto> src)
        {
            if (null == src)
            {
                return null;
            }

            return ComponentMgr.Instance.Resolve<IMapperMgr>()
                .ConvertAll<T_FromDto, T_ToDto>(src);
        }

        public virtual T GetRepository<T>(bool isSelfService = false)
            where T : class, IRepository
        {
            var repo = ComponentMgr.Instance.TryResolve<T>();
            if (null == repo)
            {
                return null;
            }

            if (null != Requester)
            {
                repo.Requester = Requester; // could be still null
            }

            if (null == repo.Requester && true == isSelfService)
            {
                repo.Requester = GetApplicationAccountId();
            }

            return repo;
        }

        public virtual T GetDomainService<T>(bool isSelfService = false)
            where T : class, IDomainService
        {
            var svc = ComponentMgr.Instance.TryResolve<T>();
            if (null == svc)
            {
                return null;
            }

            if (null != Requester)
            {
                svc.Requester = Requester; // could be still null
            }

            if (null == svc.Requester && true == isSelfService)
            {
                svc.Requester = GetApplicationAccountId();
            }

            return svc;
        }

        public virtual ISerializer GetSerializer()
        {
            if (null == m_DefaultSerializer)
            {
                lock (m_Lock)
                {
                    if (null == m_DefaultSerializer)
                    {
                        m_DefaultSerializer = ComponentMgr.Instance.TryResolve<ISerializer>()
                            ?? ComponentMgr.Instance.GetDefaultSerializer();
                    }
                }
            }

            return m_DefaultSerializer;
        }

        public virtual ICache GetCache()
        {
            if (null == m_DefaultCacheClient)
            {
                lock (m_Lock)
                {
                    if (null == m_DefaultCacheClient)
                    {
                        m_DefaultCacheClient = ComponentMgr.Instance.GetDefaultCacheFromConfig(isHealthCheck: true)
                            ?? ComponentMgr.Instance.TryResolve<ICache>()
                            ?? ComponentMgr.Instance.GetDefaultLocalCache()
                            ?? throw new OperationCanceledException("Currently cache service not available. ");
                    }
                }
            }

            return m_DefaultCacheClient;
        }

        public virtual IStorage GetStorage()
        {
            if (null == m_DefaultStorageClient)
            {
                lock (m_Lock)
                {
                    if (null == m_DefaultStorageClient)
                    {
                        m_DefaultStorageClient = ComponentMgr.Instance.GetDefaultStorageClient()
                            ?? throw new NotSupportedException("Currently storage service unavailable. ");
                    }
                }
            }

            return m_DefaultStorageClient;
        }

        public virtual INotificationSQSClient GetNotifySQSClient()
        {
            if (null == m_DefaultNotifySQSClient)
            {
                lock (m_Lock)
                {
                    if (null == m_DefaultNotifySQSClient)
                    {
                        m_DefaultNotifySQSClient = ComponentMgr.Instance.GetDefaultNotificationSQSClient()
                            ?? throw new OperationCanceledException("Currently notification service not available. ");
                    }
                }
            }

            return m_DefaultNotifySQSClient;
        }

        public virtual string GetApplicationAccountId()
        {
            if (null == m_ApplicationAccountId)
            {
                lock (m_Lock)
                {
                    if (null == m_ApplicationAccountId)
                    {
                        var profile = TokenProfileMgr.Instance.GetApiKeyProfileAsync<AccountProfileBase>(ServiceContext.ApiKey).ConfigureAwait(false).GetAwaiter().GetResult();
                        m_ApplicationAccountId = profile?.AccountId;
                    }
                }
            }

            return m_ApplicationAccountId;
        }

        public virtual Guid? GetConversationId() =>
            ServiceEntry?.GetConversationId()
                ?? _id;

        public virtual string GetRequester()
        {
            if (Requester.HasValue())
            {
                return Requester;
            }

            try
            {
                return ServiceEntry?.GetRequester(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return null;
        }

        public virtual string GetTokenAccountId()
        {
            if (Requester.HasValue())
            {
                return Requester;
            }

            try
            {
                return ServiceEntry?.GetAccountId(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return null;
        }

        public virtual TokenLevelEnum GetTokenLevelEnum()
        {
            var tokenModel = ServiceEntry?.GetTokenDetail(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            if (null != tokenModel)
            {
                if (int.TryParse(tokenModel.GetAccountLevel(), out var lv))
                {
                    return (TokenLevelEnum)lv;
                }
            }

            return TokenLevelEnum.UnSet;
        }

        public virtual string GetRequestRemoteIP()
        {
            try
            {
                return (ServiceEntry as HttpRequestServiceBase)
                    ?.Request
                    ?.RemoteIp;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return null;
        }

        public virtual string GetRequestUserAgent()
        {
            try
            {
                return CurrentRequest?.Headers?["User-Agent"];
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return null;
        }

        public IRequestService ServiceEntry { get; private set; }
        public HttpRequest CurrentRequest
        {
            get => HttpHelper.HttpContext?.Request
                ?? ServiceEntry?.CurrentRequest;
        }

        public NameValueCollection CurrentRequestHeaders
        {
            get
            {
                if (Headers?.Count > 0)
                {
                    return Headers;
                }

                if (null != HttpHelper.HttpContext?.Request?.Headers)
                {
                    return HttpHelper.HttpContext.Request.Headers.ToNameValueCollection();
                }

                if (ServiceEntry is HttpRequestServiceBase serviceEntry)
                {
                    return serviceEntry.Request?.Headers ?? Headers;
                }

                return Headers;
            }
        }

        public NameValueCollection Headers { get; set; } = new NameValueCollection();
        public string Requester { get; set; }
        public string SubCode { get; set; }
        public string SubMsg { get; set; }
        public bool IsCacheHit { get; set; }
        public bool IsValidParam { get; set; } = false;
        public int TotalCount { get; set; }
        public int ReturnCount { get; set; }

        protected static string m_ApplicationAccountId;
        protected static ISerializer m_DefaultSerializer;
        protected static ICache m_DefaultCacheClient;
        protected static IStorage m_DefaultStorageClient;
        protected static INotificationSQSClient m_DefaultNotifySQSClient;

        private static readonly object m_Lock = new object();
    }
}
