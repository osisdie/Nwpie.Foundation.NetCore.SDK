using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nwpie.MiniSite.KVS.Contract.Entities;

namespace Nwpie.MiniSite.KVS.Common.Entities.Application
{
    [Table("APPLICATION")]
    public partial class APPLICATION_Entity : KvsEntity
    {
        public APPLICATION_Entity() : base("APPLICATION") { }

        [Key]
        [StringLength(100)]
        public string app_id { get; set; }

        [StringLength(500)]
        public string sys_name { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [StringLength(2000)]
        public string description { get; set; }

        [StringLength(100)]
        public string status { get; set; }
    }
}
