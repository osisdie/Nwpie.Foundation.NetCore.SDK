﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nwpie.Foundation.Common.Serializers
{
    public class DefaultSerializer : SerializerBase
    {
        public override void Initialization()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
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
