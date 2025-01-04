using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Common.Notification;
using Nwpie.Foundation.Common.Notification.Models;
using Nwpie.Foundation.Notification.Sendgrid.Interfaces;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Nwpie.Foundation.Notification.Sendgrid
{
    public class SendGridMailService : MailBase, ISendGridMailService
    {
        public SendGridMailService(IConfigOptions<Sendgrid_Option> option)
        {
            m_Option = option
                ?? throw new ArgumentNullException(nameof(Sendgrid_Option));

            if (string.IsNullOrWhiteSpace(m_Option.Value?.Apikey))
            {
                throw new ArgumentNullException(nameof(Sendgrid_Option.Apikey));
            }

            Subject = MailConfig.SubjectPrefix;
        }

        public override void AddAttachments(string path)
        {
            var attach = File.ReadAllBytes(path);
            var attach2base64 = Convert.ToBase64String(attach);
            var myAttach = new Attachment
            {
                Content = attach2base64,
                Type = MimeTypesMap.GetMimeType(Path.GetExtension(path)),
                Filename = Path.GetFileName(path)
            };

            Attachments.Add(myAttach);
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
            file.Seek(0, SeekOrigin.Begin);

            var attach2base64 = Convert.ToBase64String(((MemoryStream)file).ToArray());
            var myAttach = new Attachment
            {
                Content = attach2base64,
                Type = MimeTypesMap.GetMimeType(Path.GetExtension(fileName)),
                Filename = Path.GetFileName(fileName)
            };

            Attachments.Add(myAttach);
        }

        public override void AddReceiver(IEnumerable<System.Net.Mail.MailAddress> someone)
        {
            Receivers.AddRange(someone.Select(x => new EmailAddress()
            {
                Email = x.Address,
                Name = x.DisplayName
            }).ToList());
        }

        public override void AddReceiver(IEnumerable<string> receivers)
        {
            foreach (var receiver in receivers)
            {
                Receivers.Add(new EmailAddress(receiver));
            }
        }

        public override void AddReceiver(System.Net.Mail.MailAddress someone)
        {
            Receivers.Add(new EmailAddress(someone.Address, someone.DisplayName));
        }

        public override async Task<IServiceResponse<bool>> SendAsync()
        {
            var result = new ServiceResponse<bool>();

            try
            {
                var sg = new SendGridClient(m_Option.Value.Apikey);
                var msg = MailHelper
                    .CreateSingleEmailToMultipleRecipients(
                        new EmailAddress(MailConfig.EmailFrom, MailConfig.DisplayName),
                        Receivers,
                        Subject,
                        string.Empty,
                        Body
                    );

                if (Attachments?.Count() > 0)
                {
                    msg.AddAttachments(Attachments);
                }

                var response = await sg.SendEmailAsync(msg);
                if ((int)response.StatusCode < (int)HttpStatusCode.Moved)
                {
                    result.Success().Content(true);
                }

                result.Msg = response.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                result.Error(StatusCodeEnum.Exception, ex);
                Logger.LogWarning(ex.ToString());
            }

            return result;
        }

        public override void Dispose()
        {

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

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public List<EmailAddress> Receivers { get; set; } = new List<EmailAddress>();

        protected IConfigOptions<Sendgrid_Option> m_Option;
    }
}
