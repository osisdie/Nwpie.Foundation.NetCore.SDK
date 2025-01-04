using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Cache.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Serializers;

namespace Nwpie.Foundation.Common.Cache
{
    public abstract class CacheBase : CObject, ICache
    {
        public CacheBase(IConfigOptions option)
        {
            m_Option = option ?? throw new ArgumentNullException(nameof(IConfigOptions));
            Serializer = new DefaultSerializer();

            Initialization();
        }

        public virtual string ConvertToRealKey(string displayKey)
        {
            var realKey = displayKey;
            if (IsAttachPrefixkeyEnabled)
            {
                realKey = displayKey.AttachPrefixToKey();
            }

            return realKey;
        }

        public virtual string ConvertToDispKey(string realKey)
        {
            var displayKey = realKey;
            if (IsAttachPrefixkeyEnabled)
            {
                displayKey = realKey.DetachPrefixFromKey();
            }

            return displayKey;
        }

        protected virtual void Initialization() { }

        public abstract Task<IServiceResponse<T>> GetAsync<T>(string displayKey);
        public abstract Task<IDictionary<string, IServiceResponse<T>>> GetAsync<T>(IList<string> displayKeys);
        public abstract Task<IServiceResponse<TResult>> GetOrSetAsync<TResult>(string displayKey, Func<TResult> func, int expiredTime = 3600)
            where TResult : class;
        public abstract Task<IServiceResponse<TResult>> GetOrSetAsync<TParam, TResult>(string displayKey, Func<TParam, TResult> func, TParam param, int expiredTime = 3600)
            where TResult : class;
        public abstract Task<IServiceResponse<bool>> RemoveAsync(string displayKey);
        public abstract Task<IDictionary<string, IServiceResponse<bool>>> RemoveAsync(IList<string> displayKeys);
        public abstract Task<IDictionary<string, IServiceResponse<bool>>> RemovePatternAsync(string displayPattern);
        public abstract Task<IServiceResponse<T>> SetAsync<T>(string displayKey, T value, int expiredTime = 3600);
        public abstract Task<IDictionary<string, IServiceResponse<T>>> SetAsync<T>(IDictionary<string, T> items, int expiredTime = ConfigConst.DefaultCacheSecs);
        public abstract Task<bool> ExistsAsync(string displayKey);

        public bool IsAttachPrefixkeyEnabled { get; set; } = true;
        public ISerializer Serializer { get; private set; }

        protected IConfigOptions m_Option { get; private set; }
    }
}
