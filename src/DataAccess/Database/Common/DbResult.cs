using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class DbResult<T, U> : DbResult<T>, IDbResult<T>, IDbResult<T, U>
        where U : new()
    {
        public DbResult()
        {
            Output = new U();
        }

        public U Output { get; set; }
    }

    public class DbResult<T> : DbResult, IDbResult<T> //where T : class, new()
    {
        public DbResult()
        {
            Data = new List<T>();
        }

        public IList<T> Data { get; set; }
    }

    public class DbResult : IDbResult
    {
        public DbResult()
        {
            IsSuccess = true;
            LastException = null;
            ReturnValue = 0;
        }

        public Exception LastException { get; set; }
        public int ReturnValue { get; set; }
        public bool IsSuccess { get; set; }
    }
}
