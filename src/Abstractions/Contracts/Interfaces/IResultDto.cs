using System;
using System.Collections.Generic;

namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IResultDto
    {
        string CacheKey { get; set; }
        string CacheProviderName { get; set; }
        bool? IsFromCache { get; set; }
        DateTime? CacheTime { get; set; }
        string SwitchedProvider { get; set; }
        string Message { get; set; }
        DateTime? _ts { get; set; }
        Dictionary<string, string> ExtensionMap { get; set; }
        bool Any();
    }

    public interface IResultDto<T> : IResultDto
    {
        List<T> Items { get; set; }
        int TotalCount { get; set; }
    }
}
