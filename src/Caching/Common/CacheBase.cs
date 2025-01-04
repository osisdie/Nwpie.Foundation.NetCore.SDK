using System;
using System.Collections.Generic;
//using Nwpie.Foundation.Common.Cache.Interfaces;
//using Nwpie.Foundation.Common.Config;
//using Nwpie.Foundation.Common.Config.Interfaces;
//using Nwpie.Foundation.Common.Contracts;
//using Nwpie.Foundation.Common.Models;

namespace Nwpie.Foundation.Caching.Common
{
    //[Obsolete("Use Nwpie.Foundation.Common.Cache.CacheBase")]
    //public abstract class CacheBase : CObject, ICache
    //{
    //    public CacheBase(IConfigOptions opt)
    //        : base()
    //    {
    //        m_Option = opt;
    //        Initialization();
    //    }

    //    protected virtual void Initialization() { }

    //    public abstract ServiceResponse<T> Get<T>(string displayKey);

    //    public abstract IDictionary<string, ServiceResponse<T>> Get<T>(IList<string> displayKeys);

    //    public abstract ServiceResponse<T> GetOrSet<T>(string displayKey, Func<T> func, int expiredTime = 3600) where T : class;

    //    public abstract ServiceResponse<TResult> GetOrSet<TParam, TResult>(string displayKey, Func<TParam, TResult> func, TParam param, int expiredTime = 3600) where TResult : class;

    //    public abstract ServiceResponse<bool> Remove(string displayKey);

    //    public abstract IDictionary<string, ServiceResponse<bool>> Remove(IList<string> displayKeys);

    //    public abstract IDictionary<string, ServiceResponse<bool>> RemovePattern(string displayPattern);

    //    public abstract ServiceResponse<T> Set<T>(string displayKey, T value, int expiredTime = 3600);

    //    public abstract IDictionary<string, ServiceResponse<T>> Set<T>(IDictionary<string, T> items, int expiredTime = ConfigConst.DefaultCacheSecs);

    //    public abstract bool Exists(string displayKey);

    //    public bool IsAttachPrefixkeyEnabled { get; set; } = true;

    //    protected IConfigOptions m_Option { get; private set; }

    //    protected readonly object m_Lock = new object();
    //}
}
