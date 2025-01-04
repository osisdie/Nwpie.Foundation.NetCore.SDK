using Nwpie.Foundation.Abstractions.Patterns;

namespace Nwpie.Foundation.Configuration.SDK
{
    public class ConfigEventMgr : SingleCObject<ConfigEventMgr>
    {
        protected override void InitialInConstructor()
        {

        }

        public virtual void OnConfigGet(string configKey, string rawData)
        {

        }

        public virtual void OnConfigRefresh(string configKey, string rawData)
        {

        }

        public override void Dispose()
        {

        }
    }
}
