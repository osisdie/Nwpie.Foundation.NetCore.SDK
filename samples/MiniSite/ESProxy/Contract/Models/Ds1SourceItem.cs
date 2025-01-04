using System;
using System.ComponentModel.DataAnnotations;

namespace Nwpie.MiniSite.ES.Contract.Models
{
    public class Ds1SourceItem
    {
        [Key]
        [StringLength(50)]
        public string itemGuid { get; set; }

        [StringLength(50)]
        public string nsRecordType { get; set; }

        [StringLength(50)]
        public string version { get; set; }

        [StringLength(255)]
        public string status { get; set; }

        public bool isDeleted { get; set; }

        public bool isHidden { get; set; }

        [StringLength(50)]
        public string nsItemId { get; set; }

        [StringLength(50)]
        public string nsInternalId { get; set; }

        public string color { get; set; }

        public DateTime? modifyAt { get; set; }

        public DateTime? createAt { get; set; }

        public string itemDescription { get; set; }

        public string cubicFeet { get; set; }

        public string netWeight { get; set; }

        public string grossWeight { get; set; }

        public string setupDimension { get; set; }

        public string setupWidth { get; set; }

        public string setupDepth { get; set; }

        public string setupHeight { get; set; }

        public string setupFootboardHeight { get; set; }

        public string boxWidth { get; set; }

        public string boxLength { get; set; }

        public string boxHeight { get; set; }

        public string upcCode { get; set; }

        public string category { get; set; }

        public string subCategory { get; set; }

        public string matWood { get; set; }

        public string matMetal { get; set; }

        public string matCushion { get; set; }

        public string matMirror { get; set; }

        public string matOthersFabric { get; set; }
    }
}
