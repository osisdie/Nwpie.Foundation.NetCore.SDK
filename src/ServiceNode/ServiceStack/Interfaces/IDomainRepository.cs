using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces
{
    public interface IDomainRepository<T> : IRepository
    {
        // Expect id is string / guid.ToString() / number.ToString() / datetime.ToString()
        Task<T> GetAsync(string id);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);

        /// Return: count(*) where condition(filter)
        /// Expect >= 0
        Task<int?> GetCountAsync();
        Task<int?> GetCountAsync(Expression<Func<T, bool>> filter);

        /// Return: list where condition(filter) offset limit
        /// Expect .Count >= 0
        Task<IEnumerable<T>> GetListAsync(int offset, int fetch, Expression<Func<T, bool>> filter);

        /// Return: inserted id
        /// <returns>id</returns>
        Task<string> InsertAsync(T entity);
        Task<string> InsertAsync(string id, T entity);

        /// Return: affected rows
        /// Expect >= 1
        Task<int?> UpdateAsync(T entity);

        /// Return: affected rows
        /// Expect >= 1
        Task<int?> RemoveAsync(T entity);
        Task<int?> RemoveAsync(string id, params string[] more);

        HttpRequest CurrentRequest { get; }
    }

    public interface IDomainRepository : IDomainRepository<object>
    {

    }
}
