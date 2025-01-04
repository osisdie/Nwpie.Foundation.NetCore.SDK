using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Patterns;

namespace Nwpie.Foundation.Caching.LocalCache
{
    public class LocalCacheMgr : SingleCObject<LocalCacheMgr>
    {
        protected override void InitialInConstructor()
        {
            var option = new ConfigOptions<LocalCache_Option>(
                new LocalCache_Option() // Not use yet
            );

            m_Client = new LocalCache(option);
        }

        public ILocalCache GetCache() => m_Client;

        public override void Dispose() { }

        protected ILocalCache m_Client;
    }
}
