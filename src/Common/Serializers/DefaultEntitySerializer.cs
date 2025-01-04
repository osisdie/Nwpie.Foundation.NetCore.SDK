using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers.Resolvers;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Common.Serializers
{
    public class DefaultEntitySerializer : SerializerBase, IEntitySerializer
    {
        public override void Initialization()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                ContractResolver = new UnderscorePropertyNamesContractResolver(),
                //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                //NamingStrategy = new SnakeCaseNamingStrategy()
            };
        }

        public override T Deserialize<T>(string serialized) =>
            JsonConvert.DeserializeObject<T>(serialized ?? string.Empty, Settings);

        public override string Serialize<T>(T deserialized) =>
            JsonConvert.SerializeObject(deserialized, Settings);

        public JsonSerializerSettings Settings { get; set; }
    }
}
