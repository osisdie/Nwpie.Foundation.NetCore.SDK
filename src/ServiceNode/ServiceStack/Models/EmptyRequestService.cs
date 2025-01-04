using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Auth.Contract.Base;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Http.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Models
{
    public class EmptyRequestService : IRequestService
    {
        public EmptyRequestService()
        {
            Logger = LogMgr.CreateLogger(GetType());
            Serializer = new DefaultSerializer();
        }

        public ILogger Logger { get; private set; }
        public Guid? Id { get; private set; } = Guid.NewGuid();
        public HttpRequest CurrentRequest { get; set; }
        public ITokenService TokenService { get; private set; }

        public async Task<ITokenDataModel> GetTokenDetail(AuthExactFlagEnum flags)
        {
            await Task.CompletedTask;
            return default(ITokenDataModel);
        }

        public Guid? GetConversationId() => Id;

        public virtual string GetRequester(AuthExactFlagEnum flags) => string.Empty;

        public async Task<T> GetProfile<T>(AuthExactFlagEnum flags) where T : AccountProfileBase
        {
            await Task.CompletedTask;
            return default(T);
        }

        public string GetAccountId(AuthExactFlagEnum flags) => string.Empty;

        public ITokenService GetTokenService() =>
            ComponentMgr.Instance.TryResolve<ITokenService>();

        public string GetRequestRemoteIP() => string.Empty;
        public string GetRequestUserAgent() => string.Empty;
        public ISerializer Serializer { get; private set; }
        public DateTime? _ts { get; private set; } = DateTime.UtcNow;
    }
}
