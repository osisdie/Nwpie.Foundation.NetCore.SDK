using Nwpie.Foundation.Abstractions.Config.Interfaces;

namespace Nwpie.Foundation.Abstractions.Config
{
    public class ConfigOptions : ConfigOptions<object>
    {
    }

    public class ConfigOptions<T> : IConfigOptions<T>
        where T : class, new()
    {
        public ConfigOptions()
        {

        }

        public ConfigOptions(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
