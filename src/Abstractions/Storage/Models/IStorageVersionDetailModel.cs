using System;

namespace Nwpie.Foundation.Abstractions.Storage.Models
{
    public interface IStorageVersionDetail
    {
        string VersionId { get; set; }
        long? Size { get; set; }
        bool? IsLatest { get; set; }
        DateTime? LastModified { get; set; }
    }
}
