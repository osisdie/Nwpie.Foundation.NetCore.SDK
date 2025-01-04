using Nwpie.Foundation.Common.Extras;

namespace Nwpie.Foundation.Common.Serializers.Extensions
{
    public static class SerializerExtension
    {
        public static string ConvertToJson(this object o) =>
            ComponentMgr.Instance.SerializerFromDI.Serialize(o);
    }
}
