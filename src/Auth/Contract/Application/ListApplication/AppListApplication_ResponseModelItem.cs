using System;

namespace Nwpie.Foundation.Auth.Contract.Application.ListApplication
{
    public class AppListApplication_ResponseModelItem
    {
        public string AppId { get; set; }
        public string SysName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }
    }
}
