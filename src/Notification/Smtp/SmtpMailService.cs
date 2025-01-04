using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Common.Notification;
using Nwpie.Foundation.Common.Notification.Models;
using Nwpie.Foundation.Notification.Smtp.Interfaces;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Notification.Smtp
{
    public class SmtpMailService : MailBase, ISmtpMailService
    {
        public SmtpMailService(IConfigOptions<Smtp_Option> option)
        {
            m_Option = option
                ?? throw new ArgumentNullException(nameof(option));

            if (null == m_Option.Value?.Host || true != m_Option.Value.Port > 0)
            {
                throw new ArgumentNullException($@"Options
                    {nameof(Smtp_Option.Host)},
                    {nameof(Smtp_Option.Port)} are required. ");
            }

            Initialize();
        }

        void Initialize()
        {
            Context = new MailMessage
            {
                From = new MailAddress(MailConfig.EmailFrom, MailConfig.DisplayName),
                IsBodyHtml = true,
            };
            Subject = MailConfig.SubjectPrefix;
        }

        public override void AddReceiver(MailAddress receiver) =>
            Receivers.Add(receiver);

        public override void AddReceiver(IEnumerable<MailAddress> receivers) =>
            Receivers.AddRange(receivers);

        public override void AddReceiver(IEnumerable<string> receivers)
        {
            foreach (var receiver in receivers)
            {
                Receivers.Add(new MailAddress(receiver));
            }
        }

        public override void AddAttachments(string path)
        {
            var memStream = new MemoryStream();
            {
                using (var fileStream = new FileStream(path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete))
                {
                    var bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);
                    memStream.Write(bytes, 0, (int)fileStream.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    Context.Attachments
                        .Add(new Attachment(memStream, Path.GetFileName(path)));
                    fileStream.Dispose();
                }
            }
        }

        public override void AddAttachments(byte[] file, string fileName)
        {
            using (var fileMem = new MemoryStream(file))
            {
                AddAttachments(fileMem, fileName);
            }
        }

        public override void AddAttachments(Stream file, string fileName)
        {
            Attachments.Add(new Attachment(file, fileName));
            //Context.Attachments.Add(new Attachment(file, fileName));
        }

        public override async Task<IServiceResponse<bool>> SendAsync()
        {
            var result = new ServiceResponse<bool>();

            try
            {
                Context.Subject = Subject;
                Context.Body = Body;
                using (var smtp = new SmtpClient())
                {
                    if (Attachments?.Count() > 0)
                    {
                        Attachments.ForEach(o =>
                        {
                            Context.Attachments.Add(o);
                        });
                    }

                    if (m_Option.Value.User.HasValue() &&
                        m_Option.Value.Pass.HasValue())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = m_Option.Value.User,
                            Password = m_Option.Value.Pass
                        };

                        smtp.Credentials = credential;
                    }
                    smtp.Host = m_Option.Value.Host;
                    smtp.Port = m_Option.Value.Port.Value;
                    smtp.EnableSsl = true; // TODO: Use Option.Value.EnableSSL

                    foreach (var receiver in Receivers)
                    {
                        Context.To.Clear();
                        Context.To.Add(receiver);
                        await smtp.SendMailAsync(Context);
                    }

                    result.Success().Content(true);
                }

            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, ex);
                Logger.LogError(ex.ToString());
            }

            return result;
        }

        public override void Dispose()
        {
            Context?.Dispose();
        }

        protected string m_Subject = "";
        public override string Subject
        {
            get => m_Subject;
            set => m_Subject = value?.Trim();
        }

        protected string m_Body = "";
        public override string Body
        {
            get => m_Body;
            set => m_Body = value
                ?.Replace("\r\n", "<br />")
                ?.Replace("\r", "<br />")
                ?.Replace("\n", "<br />")
                + Environment.NewLine;
        }

        protected string m_Template = "";
        public string Template
        {
            get => m_Template;
            set => m_Template = value;
        }

        public MailMessage Context { get; set; }
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public List<MailAddress> Receivers { get; set; } = new List<MailAddress>();

        protected IConfigOptions<Smtp_Option> m_Option;
    }
}
