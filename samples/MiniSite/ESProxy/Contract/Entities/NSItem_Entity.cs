namespace Nwpie.MiniSite.ES.Contract.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Nwpie.Foundation.Abstractions.DataAccess.Models;

    [Table("NSItem")]
    public partial class NSItem_Entity : EntityBase
    {
        public NSItem_Entity() : base("NSItem") { }

        [Key]
        [StringLength(50)]
        public string nsitemId { get; set; }

        [StringLength(50)]
        public string cubicFeet { get; set; }

        [StringLength(50)]
        public string netWeight { get; set; }

        [StringLength(50)]
        public string grossWeight { get; set; }

        [StringLength(50)]
        public string setupDimension { get; set; }

        [StringLength(50)]
        public string setupWidth { get; set; }

        [StringLength(50)]
        public string setupDepth { get; set; }

        [StringLength(50)]
        public string setupHeight { get; set; }

        [StringLength(50)]
        public string setupFootboardHeight { get; set; }

        [StringLength(50)]
        public string boxWidth { get; set; }

        [StringLength(50)]
        public string boxLength { get; set; }

        [StringLength(50)]
        public string boxHeight { get; set; }

        [StringLength(2000)]
        public string description { get; set; }

        public int sort { get; set; }

        public bool isDel { get; set; }

        public bool isHidden { get; set; }

        [StringLength(50)]
        public string modifier { get; set; }

        public DateTime? updated { get; set; }

        [StringLength(50)]
        public string creator { get; set; }

        public DateTime? created { get; set; }
    }
}
