using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nwpie.Foundation.Common.Serializers
{
    public class DefaultJsonConfigSerializer : SerializerBase, IJsonConfigSerializer
    {
        public override void Initialization()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public override T Deserialize<T>(string serialized)
        {
            return JsonConvert
                .DeserializeObject<T>(serialized ?? string.Empty, Settings);
        }

        public override string Serialize<T>(T deserialized)
        {
            return JsonConvert
                .SerializeObject(deserialized, Settings);
        }

        public JsonSerializerSettings Settings { get; set; }
    }
}
