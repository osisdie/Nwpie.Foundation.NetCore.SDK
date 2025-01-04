using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nwpie.MiniSite.KVS.Contract.Entities;

namespace Nwpie.MiniSite.KVS.Common.Entities.Permission
{
    [Table("PERMISSION")]
    public partial class PERMISSION_Entity : KvsEntity
    {
        public PERMISSION_Entity() : base("PERMISSION") { }

        [Key]
        [StringLength(100)]
        public string perm_id { get; set; }

        [StringLength(500)]
        public string sys_name { get; set; }

        [StringLength(200)]
        public string name { get; set; }

        [StringLength(2000)]
        public string script { get; set; }

        public int? weight { get; set; }

        [StringLength(2000)]
        public string description { get; set; }

        [StringLength(100)]
        public string status { get; set; }
    }
}
