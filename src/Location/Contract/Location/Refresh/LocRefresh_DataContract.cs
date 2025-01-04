using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Location.Refresh
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("RefreshApiLocation")]
    [Route(LocationConst.HttpPathToLocationContractRequest_Refresh, "GET,POST")]
    public class LocRefresh_Request : IServiceReturn<bool>
    {
    }

    #endregion
}
