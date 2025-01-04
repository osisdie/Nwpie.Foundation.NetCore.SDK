
using System.Text.Json;

namespace Nwpie.Foundation.Common.Serializers
{
    public class SysTextJsonSerializer : SerializerBase
    {
        public override void Initialization()
        {
            Settings = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //IgnoreNullValues = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Always,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public override T Deserialize<T>(string serialized) =>
            JsonSerializer.Deserialize<T>(serialized ?? string.Empty, Settings);

        public override string Serialize<T>(T deserialized) =>
            JsonSerializer.Serialize(deserialized, Settings);

        public JsonSerializerOptions Settings { get; set; }
    }
}
