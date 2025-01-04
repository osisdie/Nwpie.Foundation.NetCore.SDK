using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;

namespace Nwpie.Foundation.Abstractions.Mappers.Interfaces
{
    public interface IMapperMgr : ISingleCObject
    {
        TDestination ConvertTo<TDestination>(object source);
        IEnumerable<TDestination> ConvertAll<TDestination>(IEnumerable<object> source);
        TDestination ConvertTo<TSource, TDestination>(TSource source);
        IEnumerable<TDestination> ConvertAll<TSource, TDestination>(IEnumerable<TSource> source);
    }
}
