using System;
using Nwpie.Foundation.Http.Common.Interfaces;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces
{
    public interface IRepository
    {
        Guid? GetConversationId();
        string GetRequester();
        string GetTokenAccountId();
        string GetRequestRemoteIP();

        IRequestService ServiceEntry { get; }
        string Requester { get; set; }
    }
}
