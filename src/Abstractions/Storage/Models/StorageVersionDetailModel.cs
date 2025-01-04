using System;

namespace Nwpie.Foundation.Abstractions.Storage.Models
{
    public class StorageVersionDetailModel : IStorageVersionDetail
    {
        public string VersionId { get; set; } // "null" is a special Id
        public long? Size { get; set; }
        public bool? IsLatest { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
