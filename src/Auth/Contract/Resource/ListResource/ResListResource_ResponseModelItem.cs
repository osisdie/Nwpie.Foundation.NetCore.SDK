using System;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListResource
{
    public class ResListResource_ResponseModelItem
    {
        public string OwnerAccountId { get; set; }
        public string AccountId { get; set; }
        public string ObjId { get; set; }
        public string SrcId { get; set; }
        public string PermId { get; set; }
        public string DisplayName { get; set; }
        public string Tags { get; set; } // json
        public string Metadata { get; set; } // json
        public bool? IsBlocked { get; set; }
        public bool? IsFunc { get; set; }
        public string SrcPath { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }
        public string ObjUrl { get; set; } // https url
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
    }
}
