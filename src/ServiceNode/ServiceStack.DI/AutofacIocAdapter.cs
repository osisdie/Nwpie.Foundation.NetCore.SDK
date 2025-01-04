using System;
using Autofac;
using ServiceStack.Configuration;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.DI
{
    public class AutofacIocAdapter : IContainerAdapter
    {
        //public AutofacIocAdapter(Autofac.IContainer container)
        //{
        //    m_Container = container
        //        ?? throw new ArgumentNullException(nameof(Autofac.IContainer));
        //}

        public AutofacIocAdapter(Autofac.ILifetimeScope container)
        {
            m_Container = container
                ?? throw new ArgumentNullException(nameof(Autofac.ILifetimeScope));
        }

        public T Resolve<T>()
        {
            var component = TryResolve<T>();
            return false == component.Equals(default(T))
                ? component
                : throw new Exception($"Error trying to resolve '{typeof(T).Name}' ");
        }

        public T TryResolve<T>()
        {
            //if (m_Container.TryResolve<Autofac.ILifetimeScope>(out var scope) &&
            //    scope.TryResolve(typeof(T), out var scopeComponent))
            //{
            //    return (T)scopeComponent;
            //}

            if (m_Container.TryResolve(typeof(T), out var component))
            {
                return (T)component;
            }

            return default(T);
        }

        //protected readonly Autofac.IContainer m_Container;
        protected readonly Autofac.ILifetimeScope m_Container;
    }
}
