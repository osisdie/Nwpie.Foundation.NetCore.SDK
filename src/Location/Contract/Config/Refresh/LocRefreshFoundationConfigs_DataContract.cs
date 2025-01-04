using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Config.Refresh
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("RefreshFoundationConfigs")]
    [Route(LocationConst.HttpPathToConfigContractRequest_Refresh, "GET,POST")]
    public class LocRefreshFoundationConfigs_Request : IServiceReturn<IDictionary<string, bool>>
    {
        /// <summary>
        /// Search config array
        /// </summary>
        public List<string> ConfigKeys { get; set; }

        /// <summary>
        /// (optional) Prefix
        /// </summary>
        public string Prefix { get; set; }
    }

    #endregion
}
