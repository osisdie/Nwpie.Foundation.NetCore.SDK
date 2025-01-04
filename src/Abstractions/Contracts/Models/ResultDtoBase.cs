using System;
using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Abstractions.Contracts.Models
{
    public class ResultDtoBase : IResultDto
    {
        /// <summary>
        /// CacheKey if data is from Cache
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// CacheProvider if data is from Cache
        /// </summary>
        public string CacheProviderName { get; set; }

        /// <summary>
        /// true if data is from Cache, else false
        /// </summary>
        public bool? IsFromCache { get; set; }

        /// <summary>
        /// Cache created time if data is from Cache
        /// </summary>
        public DateTime? CacheTime { get; set; }

        /// <summary>
        /// Replica node name (if exists)
        /// </summary>
        public string SwitchedProvider { get; set; }

        /// <summary>
        /// Extension Dictionary, KV
        /// </summary>
        //[ApiMember(Description = "Extension Dictionary, KV")]
        public Dictionary<string, string> ExtensionMap { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string Message { get; set; }

        /// <summary>
        /// System timestamp
        /// </summary>
        public DateTime? _ts { get; set; }

        public virtual bool Any() => true;
    }

    //public class DomainData<T> : DomainResultData
    public class ResultDtoBase<T> : ResultDtoBase, IResultDto<T>
    {
        /// <summary>
        /// Array result data
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Total data count on server, for paging use
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Safely check if data exists, after IsSuccess = true
        /// </summary>
        /// <returns></returns>
        public override bool Any() => Items?.Count() > 0;
    }
}
