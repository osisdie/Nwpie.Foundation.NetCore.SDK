using System;
using System.ComponentModel.DataAnnotations.Schema;
using Nwpie.Foundation.Abstractions.DataAccess.Models;

namespace Nwpie.xUnit.Models
{
    [Table("TestTable")]
    public class TestTable_Entity : EntityBase
    {
        public TestTable_Entity() : base("TestTable") { }

        public int Id { get; set; }
        public string ColumnChar { get; set; }
        public int? ColumnInt { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public bool? ColumnBool { get; set; }
        public DateTime? ColumnDate { get; set; }
        public DateTime? ColumnDatetime { get; set; }
        public string ColumnNotExists { get; set; }

    }
}
