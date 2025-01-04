using System.Runtime.Serialization;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.Contract.Location.GetLocation
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("GetApiLocation")]
    [Route("/Location/GetApiLocation", "GET,POST")]
    public class LocGetApiLocation_Request : IServiceReturn<string>
    {
        /// <summary>
        /// Search url by target app (deployment name)
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Which application you are
        /// </summary>
        public EnvInfo EnvInfo { get; set; }

        /// <summary>
        /// Get Internal Url first
        /// </summary>
        public bool IsInternalFirst { get; set; }

        #region Hidden fields
        /// <summary>
        /// Which application you are
        /// </summary>
        [IgnoreDataMember]
        public string ApiKey { get; set; }
        #endregion
    }

    #endregion
}
