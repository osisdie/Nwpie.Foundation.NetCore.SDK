using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using ServiceStack;

namespace Nwpie.Foundation.S3Proxy.Contract.Upload
{
    public class ImageUpload_RequestModel : ResultDtoBase
    {
        /// <summary>
        /// S3 bucket name WITHOUT slash
        /// </summary>
        [Required]
        [ApiMember(Description = "S3 bucket name WITHOUT slash", IsRequired = true)]
        public string Bucket { get; set; }

        /// <summary>
        /// S3 Path WITHOUT bucket name
        /// </summary>
        [Required]
        [ApiMember(Description = "S3 Path WITHOUT bucket name", IsRequired = true)]
        public string FileKey { get; set; }
    }
}
