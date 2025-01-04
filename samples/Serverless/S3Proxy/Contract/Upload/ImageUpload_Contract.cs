using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Location;
using ServiceStack;

namespace Nwpie.Foundation.S3Proxy.Contract.Upload
{
    #region Data contract

    /// <summary>
    /// Request DTO.
    /// </summary>
    [Api("ImageUpload")]
    [Route(LocationConst.HttpPathToS3ProxyContractRequest_Upload, "GET,PUT,POST")]
    public class ImageUpload_Request :
        ImageUpload_RequestModel,
        IServiceReturn<ImageUpload_Response>
    {

    }

    /// <summary>
    /// Response DTO.
    /// </summary>
    public class ImageUpload_Response :
        ContractResponseBase<ImageUpload_ResponseModel>
    {

    }
    #endregion
}
