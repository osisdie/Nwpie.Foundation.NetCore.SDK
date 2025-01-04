using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Location.Refresh
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("RefreshAppEnvIpMapping")]
    [Route("/Location/RefreshAppEnvIpMapping", "GET,POST")]
    public class LocRefreshAppEnvIpMapping_Request : IServiceReturn<bool>
    {
    }

    #endregion
}
