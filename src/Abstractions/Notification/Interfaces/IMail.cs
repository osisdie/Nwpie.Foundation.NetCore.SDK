using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;

namespace Nwpie.Foundation.Abstractions.Notification.Interfaces
{
    public interface IMail : ICObject, IDisposable
    {
        void AddAttachments(string path);
        void AddAttachments(byte[] file, string fileName);
        void AddAttachments(Stream file, string fileName);
        void AddReceiver(System.Net.Mail.MailAddress receiver);
        void AddReceiver(IEnumerable<string> receivers);
        void AddReceiver(IEnumerable<System.Net.Mail.MailAddress> receivers);
        Task<IServiceResponse<bool>> SendAsync();

        string Body { get; }
        string Subject { get; }
    }
}
