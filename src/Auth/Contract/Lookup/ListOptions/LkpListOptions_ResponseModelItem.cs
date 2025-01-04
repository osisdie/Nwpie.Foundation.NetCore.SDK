using System;

namespace Nwpie.Foundation.Auth.Contract.Lookup.ListOptions
{
    public class LkpListOptions_ResponseModelItem
    {
        public string Category { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Sort { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Created { get; set; }
        public string Modifier { get; set; }
        public string Creator { get; set; }
    }
}
