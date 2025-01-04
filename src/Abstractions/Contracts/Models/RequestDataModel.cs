using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Abstractions.Utilities;

namespace Nwpie.Foundation.Abstractions.Contracts.Models
{
    public class RequestDataModel : RequestDataModel<object>
    {

    }

    public class RequestDataModel<T> : IRequestMessage<T>
    {
        public T Data { get; set; }
        public Dictionary<string, string> ExtensionMap { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public virtual bool Validate()
        {
            if (EqualityComparer<T>.Default.Equals(Data, default))
            {
                if (this is IRequestDataNotAllowDefault &&
                    (false == typeof(T).IsValueTyped() || typeof(T).IsNullable()))
                {
                    ExtensionMap[CommonConst.ValidateMsgKey] = $"{nameof(Data)} is required. ";
                    return false;
                }

                return true; // ignore
            }

            return ValidateAndFillMessage(out _);
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

        public virtual bool ValidateAndFillMessage(out List<string> msgList)
        {
            var result = ValidateUtils.Validate(this);
            msgList = result.Data;
            if (msgList?.Count() > 0)
            {
                ExtensionMap[CommonConst.ValidateMsgKey] = string.Join(" ", msgList);
            }

            return result.IsSuccess;
        }

        public virtual TClone Convert4Cache<TClone>()
            where TClone : RequestDataModel, new()
        {
            var t = MemberwiseClone() as TClone; //Make sure there is no Ref type in the this object
            t.ExtensionMap = null;
            return t;
        }
    }
}
