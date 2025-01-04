using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.Contract.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Http.Common.Interfaces
{
    public interface IRequestService
    {
        Task<ITokenDataModel> GetTokenDetail(AuthExactFlagEnum flags);
        string GetRequester(AuthExactFlagEnum flags); // GetTokenDetail().AccountId
        string GetAccountId(AuthExactFlagEnum flags); // GetTokenDetail().AccountId
        Task<T> GetProfile<T>(AuthExactFlagEnum flags) where T : AccountProfileBase;
        //ITokenService GetTokenService();
        Guid? GetConversationId();
        string GetRequestRemoteIP(); // // GetTokenDetail().RemoteIP
        string GetRequestUserAgent(); // // GetTokenDetail().UserAgent

        ILogger Logger { get; }
        ISerializer Serializer { get; }
        Guid? Id { get; }
        HttpRequest CurrentRequest { get; }
    }
}
