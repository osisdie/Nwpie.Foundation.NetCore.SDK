using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Notification.Interfaces;

namespace Nwpie.Foundation.Common.Notification
{
    public abstract class MailBase : CObject, IMail
    {
        public MailBase() { }

        public abstract void AddAttachments(string path);
        public abstract void AddAttachments(byte[] file, string fileName);
        public abstract void AddAttachments(Stream file, string fileName);
        public abstract void AddReceiver(MailAddress someone);
        public abstract void AddReceiver(IEnumerable<string> someone);
        public abstract void AddReceiver(IEnumerable<MailAddress> someone);
        public abstract Task<IServiceResponse<bool>> SendAsync();
        public abstract void Dispose();
        public virtual string Body { get; set; }
        public virtual string Subject { get; set; }
    }
}
