using System.ComponentModel.DataAnnotations;

namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.Upload
{
    /// <summary>
    /// Request DTO.
    /// </summary>
    //[Api("ImageUpload")]
    //[Route("/S3Proxy/Upload", "GET,POST")]
    public class ImageUpload_Request
    {
        /// <summary>
        /// S3 Bucket
        /// </summary>
        [Required]
        public string Bucket { get; set; }

        /// <summary>
        /// S3 Path WITHOUT bucket name
        /// </summary>
        [Required]
        public string FileKey { get; set; }
    }
}
