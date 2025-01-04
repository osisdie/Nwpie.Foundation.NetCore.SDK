using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Abstractions.Contracts.Models
{
    public abstract class RequestDtoBase : IRequestDto
    {
        /// <summary>
        /// Paging index, start from 0 if needed
        /// </summary>
        public virtual int? PageIndex { get; set; }

        /// <summary>
        /// Paging size, start from 1 if needed
        /// </summary>
        public virtual int? PageSize { get; set; }

        public virtual List<OrderByItem> Sorts { get; set; }
        public virtual List<string> Fields { get; set; }

        public Dictionary<string, string> ExtensionMap { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public virtual bool Validate()
        {
            var result = ValidateUtils.Validate(this);
            var msgList = result.Data;
            if (msgList?.Count() > 0)
            {
                ExtensionMap[CommonConst.ValidateMsgKey] = string.Join(" ", msgList);
            }

            return result.IsSuccess;

        }
        public virtual bool ValidateAndThrow()
        {
            var isValid = Validate();
            if (false == isValid)
            {
                throw new ArgumentException(ExtensionMap[CommonConst.ValidateMsgKey]);
            }

            return isValid;
        }

        public virtual TClone Convert4Cache<TClone>()
            where TClone : RequestDtoBase, new()
        {
            var t = MemberwiseClone() as TClone; //Make sure there is no Ref type in the this object
            t.ExtensionMap = null;
            return t;
        }
    }
}
