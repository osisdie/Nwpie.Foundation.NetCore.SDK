using System.Collections;
using Nwpie.Foundation.Abstractions.Attributes;

namespace Nwpie.Foundation.Abstractions.Validation.Attributes
{
    public class NotEmptyArrayAttribute : ValidationAttributeBase
    {
        public override bool IsValid(object value)
        {
            if (value is ICollection collection)
            {
                return collection.Count > 0;
            }

            return false;
        }
    }
}
