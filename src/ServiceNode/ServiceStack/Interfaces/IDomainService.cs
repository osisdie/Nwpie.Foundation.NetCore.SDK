using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Storage.Interfaces;
using Nwpie.Foundation.Http.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces
{
    public interface IDomainService
    {
        bool Validate(object param);// where T : RequestDtoBase;
        bool ValidateAndThrow(object param);// where T : RequestDtoBase;
        void ValidatePagingNumber(RequestDtoBase param);
        void FillCacheInfo<T>(T responseDto, string cacheKey) where T : ResultDtoBase;
        void WrapBadResponseAndThrow(IServiceResponse responseDto);
        void ClearResult();
        T_ToDto ConvertTo<T_FromDto, T_ToDto>(T_FromDto src);
        IEnumerable<T_ToDto> ConvertAll<T_FromDto, T_ToDto>(IEnumerable<T_FromDto> src);

        T GetRepository<T>(bool isSelfService = false) where T : class, IRepository;
        T GetDomainService<T>(bool isSelfService = false) where T : class, IDomainService;

        ISerializer GetSerializer();
        ICache GetCache();
        IStorage GetStorage();
        INotificationSQSClient GetNotifySQSClient();
        Guid? GetConversationId();
        string GetRequester();
        string GetTokenAccountId();
        string GetRequestRemoteIP();
        string GetRequestUserAgent();
        HttpRequest CurrentRequest { get; }
        IRequestService ServiceEntry { get; }
        NameValueCollection Headers { get; set; }
        string Requester { get; set; }
        string SubCode { get; set; }
        string SubMsg { get; set; }
        bool IsValidParam { get; set; }
        bool IsCacheHit { get; set; }
        int TotalCount { get; set; }
        int ReturnCount { get; set; }
    }
}
