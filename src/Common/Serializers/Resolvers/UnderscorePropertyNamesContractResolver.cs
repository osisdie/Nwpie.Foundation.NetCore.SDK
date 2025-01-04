using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace Nwpie.Foundation.Common.Serializers.Resolvers
{
    public class UnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public UnderscorePropertyNamesContractResolver() : base()
        {
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex
                .Replace(propertyName, @"(\w)([A-Z])", "$1_$2")
                .ToLower();
        }
    }
}
