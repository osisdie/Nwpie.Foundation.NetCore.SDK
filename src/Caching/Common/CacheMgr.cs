using Autofac;
//using Nwpie.Foundation.Common.Cache.Interfaces;
//using Nwpie.Foundation.Common.Config.Interfaces;
//using Nwpie.Foundation.Common.Extras.Interfaces;
//using Nwpie.Foundation.Common.Models;
using System;
using System.Linq;
using System.Reflection;

namespace Nwpie.Foundation.Caching.Common
{
    //public class CacheMgr : CObject, ISingleCObject
    //{
    //    public IContainer RegisterServices(params Assembly[] assembliesWithCacheProviders)
    //    {
    //        m_Assembly = assembliesWithCacheProviders ?? throw new ArgumentNullException(nameof(Assembly));

    //        var builder = new ContainerBuilder();
    //        builder.RegisterAssemblyTypes(assembliesWithCacheProviders)
    //            .PublicOnly()
    //            .Where(t => false == t.IsInterface)
    //            .As<ICache>()
    //            .AsImplementedInterfaces()
    //            .SingleInstance();

    //        m_Container = builder.Build();
    //        return m_Container;
    //    }

    //    [Obsolete("Use ComponentMgr.Instance.GetCache")]
    //    public T GetCache<T>(IConfigOptions option) where T : ICache
    //    {
    //        if (null == option)
    //        {
    //            throw new ArgumentNullException(nameof(option));
    //        }

    //        try
    //        {
    //            if (0 == m_Assembly.Length ||
    //                false == m_Assembly.Contains(typeof(T).Assembly))
    //            {
    //                m_Assembly = m_Assembly.Concat(new Assembly[] { typeof(T).Assembly }).ToArray();
    //                RegisterServices(m_Assembly);
    //            }

    //            return (T)m_Container.Resolve(typeof(T),
    //                new TypedParameter(option.GetType(), option));
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception($"Missing {typeof(T).Name}'s assembly or arguments", ex);
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        if (null != m_Container)
    //        {
    //            m_Container.Dispose();
    //        }
    //    }

    //    protected Autofac.IContainer m_Container;
    //    protected Assembly[] m_Assembly = new Assembly[] { };
    //}
}
