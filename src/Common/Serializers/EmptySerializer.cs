using System;

namespace Nwpie.Foundation.Common.Serializers
{
    public class EmptySerializer : SerializerBase
    {
        public override void Initialization() { }

        // Expect same type of input
        // Or get default if failed to convert
        public override T Deserialize<T>(string serialized)
        {
            try
            {
                return (T)Convert.ChangeType(serialized ?? string.Empty, typeof(T));
            }
            catch { return default(T); }
        }

        public override string Serialize<T>(T deserialized) => deserialized?.ToString();
    }
}
