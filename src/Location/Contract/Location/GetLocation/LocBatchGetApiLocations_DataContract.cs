using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Location.GetLocation
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("BatchGetApiLocations")]
    [Route("/Location/BatchGetApiLocations", "GET,POST")]
    public class LocBatchGetApiLocations_Request :
        IServiceReturn<IDictionary<string, string>>
    {
        /// <summary>
        /// Which application you are
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Search url by target app (deployment name)
        /// </summary>
        public List<string> AppNames { get; set; }

        /// <summary>
        /// Which application you are
        /// </summary>
        public EnvInfo EnvInfo { get; set; }

        /// <summary>
        /// Get Internal Url first
        /// </summary>
        public bool IsInternalFirst { get; set; }
    }

    #endregion
}
