using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Location.Refresh
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("RefreshEnvs")]
    [Route("/Location/RefreshEnvs", "GET,POST")]
    public class LocRefreshEnvs_Request : IServiceReturn<IDictionary<string, ServiceEnvironment>>
    {
    }

    #endregion
}
