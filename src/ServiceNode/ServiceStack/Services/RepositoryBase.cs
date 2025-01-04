using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Models;
using Nwpie.Foundation.Http.Common.Interfaces;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Services
{
    public abstract class RepositoryBase<T> : CObject,
        IDomainRepository<T>
    {
        public RepositoryBase()
        {
            if (null != HttpHelper.HttpContext?.Items)
            {
                if (HttpHelper.HttpContext.Items.TryGetValue(nameof(IRequestService), out var s) &&
                    s is IRequestService svc)
                {
                    ServiceEntry = svc;
                }
            }

            Initialize();
        }

        void Initialize()
        {
            //if (null == UseTables)
            //{
            //    UseTables = new ConcurrentDictionary<string, bool>();

            //    _AddCacheTables(typeof(T_Entity).GetCustomAttributes(typeof(TableAttribute), true));
            //    _AddCacheTables(GetType().GetCustomAttributes(typeof(TableAttribute), true));
            //}

            Initialization();
        }

        public virtual void Initialization() { }

        public virtual Task<T> GetAsync(string id) =>
            throw new NotImplementedException();
        public virtual Task<T> GetAsync(Expression<Func<T, bool>> filter) =>
            throw new NotImplementedException();
        public virtual Task<int?> GetCountAsync() =>
            throw new NotImplementedException();
        public virtual Task<int?> GetCountAsync(Expression<Func<T, bool>> filter) =>
            throw new NotImplementedException();
        public virtual Task<IEnumerable<T>> GetListAsync(int offset, int fetch, Expression<Func<T, bool>> filter) =>
            throw new NotImplementedException();
        public virtual Task<string> InsertAsync(T entity) =>
            throw new NotImplementedException();
        public virtual Task<string> InsertAsync(string id, T entity) =>
            throw new NotImplementedException();
        public virtual Task<int?> UpdateAsync(T entity) =>
            throw new NotImplementedException();
        public virtual Task<int?> RemoveAsync(T entity) =>
            throw new NotImplementedException();
        public virtual Task<int?> RemoveAsync(string id, params string[] more) =>
            throw new NotImplementedException();

        protected void AddCacheTables(object[] attributes)
        {
            if (attributes?.Count() > 0)
            {
                foreach (var attr in attributes)
                {
                    if (attr is TableAttribute customAttribute)
                    {
                        //UseTables.TryAdd(customAttribute.Name, true);
                    }
                }
            }
        }

        public Guid? GetConversationId() =>
            ServiceEntry?.GetConversationId()
            ?? _id;

        public virtual string GetRequester()
        {
            if (Requester.HasValue())
            {
                return Requester;
            }

            return ServiceEntry?.GetRequester(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString);
        }

        public virtual string GetTokenAccountId()
        {
            if (Requester.HasValue())
            {
                return Requester;
            }

            return ServiceEntry?.GetAccountId(AuthExactFlagEnum.ApiKeyHeader | AuthExactFlagEnum.AuthorizationHeader | AuthExactFlagEnum.TokenQueryString);
        }

        public virtual string GetRequestRemoteIP() =>
            (ServiceEntry as HttpRequestServiceBase)
            ?.Request?.RemoteIp
            ?? string.Empty;

        public HttpRequest CurrentRequest
        {
            get => HttpHelper.HttpContext?.Request
                ?? ServiceEntry?.CurrentRequest;
        }

        public IRequestService ServiceEntry { get; private set; }
        public string Requester { get; set; } // = null;
    }

    //public abstract class RepositoryBase : RepositoryBase<EntityBase>
    public abstract class RepositoryBase : RepositoryBase<object>
    {
    }
}
