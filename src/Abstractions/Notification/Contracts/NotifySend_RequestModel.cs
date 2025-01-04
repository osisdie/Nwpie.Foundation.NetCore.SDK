using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using ServiceStack;

namespace Nwpie.Foundation.Abstractions.Notification.Contracts
{
    public class NotifySend_RequestModel : RequestDtoBase
    {
        #region required property
        /// <summary>
        /// @reference NotifyChannelEnum
        ///     1: Smtp, 2: Sms, 3: Line, 4: Slack, 5: WebHook
        /// </summary>
        [Required]
        [ApiMember(IsRequired = true)]
        public virtual byte? Kind { get; set; }

        [Required]
        [ApiMember(IsRequired = true)]
        public virtual string Message { get; set; }

        /// <summary>
        /// Example:
        ///     Email List : a1@mail,a2@mail,
        ///     Sms List: 0912345678,0987654321
        ///     Line List: userId or groupId or roomId
        ///     Slack List: channel
        /// </summary>
        [Required]
        [ApiMember(IsRequired = true)]
        public virtual string ToList { get; set; }

        /// <summary>
        /// Alternative channel if main kind failed
        ///     Key: Channel byte
        ///     Value: Notify receivers string
        /// </summary>
        public virtual List<KeyValuePair<byte, string>> OrKinds { get; set; }

        /// <summary>
        /// Also notify to other channels
        ///     Key: Channel byte
        ///     Value: Notify receivers string
        /// </summary>
        public virtual List<KeyValuePair<byte, string>> AndKinds { get; set; }

        /// <summary>
        /// Notice that {{Title}} will be replaced in Development mode
        /// Example:
        ///     Debug mode becomes: "[debug] your original title"
        ///     Development becomes: "[dev] your original title"
        ///     Production: NO any changes
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Null: default
        /// </summary>
        public virtual string TemplateId { get; set; }

        /// <summary>
        /// Example:
        ///     Hi, {{Greeting}}
        /// </summary>
        public virtual string Greeting { get; set; }
        #endregion

        #region reserved property

        public virtual string Sender { get; set; }

        /// <summary>
        /// Audit guid, check status
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Audit
        /// </summary>
        public string ApiName { get; set; }
        public string ApiKey { get; set; }

        /// <summary>
        /// Process priority
        ///     0.normal (default)
        /// </summary>
        public byte? Priority { get; set; }

        public DateTime? SendTime { get; set; }
        #endregion
    }
}
