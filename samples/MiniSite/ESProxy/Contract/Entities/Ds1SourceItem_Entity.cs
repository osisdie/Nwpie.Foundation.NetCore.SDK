using System;
using System.ComponentModel.DataAnnotations;

namespace Nwpie.MiniSite.ES.Contract.Entities
{
    public class Ds1SourceItem_Entity
    {
        [Key]
        [StringLength(50)]
        public string item_guid { get; set; }

        [StringLength(50)]
        public string group_id { get; set; }

        [StringLength(50)]
        public string ns_record_type { get; set; }

        [StringLength(50)]
        public string version { get; set; }

        [StringLength(255)]
        public string status { get; set; }

        public bool is_deleted { get; set; }

        public bool is_hidden { get; set; }

        [StringLength(50)]
        public string ns_item_id { get; set; }

        [StringLength(50)]
        public string ns_internal_id { get; set; }

        public string color { get; set; }

        public DateTime? modify_at { get; set; }

        public DateTime? create_at { get; set; }

        public string ns_data { get; set; }
    }
}
