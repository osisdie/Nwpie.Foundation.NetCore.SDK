using System;

namespace Nwpie.Foundation.Auth.Contract.Resource.ListPermission
{
    public class ResListPermission_ResponseModelItem
    {
        public string Status { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
    }
}
