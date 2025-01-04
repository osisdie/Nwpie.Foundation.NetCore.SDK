using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Location.GetLocation
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("GetAllLocationConfig")]
    [Route("/Location/GetAllLocationConfig", "GET,POST")]
    public class LocGetAllLocationConfig_Request :
        IServiceReturn<IDictionary<string, List<string>>>
    {

    }

    #endregion
}
