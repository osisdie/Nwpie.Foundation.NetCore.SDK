using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IDbResult<T, U> : IDbResult<T>, IDbResult
        where U : new()
    {
        U Output { get; set; }
    }
    public interface IDbResult<T> : IDbResult
    {
        IList<T> Data { get; set; }
    }

    public interface IDbResult
    {
        Exception LastException { get; set; }
        int ReturnValue { get; set; }
        bool IsSuccess { get; set; }
    }
}
