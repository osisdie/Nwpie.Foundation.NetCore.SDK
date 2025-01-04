using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Config.Refresh
{
    public class LocConfigRefresh_RequestModel : RequestDtoBase
    {
        [Required]
        [NotEmptyArray]
        [ApiMember(Description = "Array of configKey", IsRequired = true)]
        public List<string> ConfigKeys { get; set; }

        /// <summary>
        /// (optional) Prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// (optional) Pull latest to cache at the same time
        /// </summary>
        public bool? PullLatest { get; set; }
    }
}
