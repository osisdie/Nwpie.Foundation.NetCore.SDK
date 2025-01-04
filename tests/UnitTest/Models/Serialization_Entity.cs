using System;
using System.ComponentModel.DataAnnotations.Schema;
using Nwpie.Foundation.Abstractions.DataAccess.Models;

namespace Nwpie.xUnit.Models
{
    [Table("TestTable")]
    public class CamelEntity : EntityBase
    {
        public CamelEntity() : base("TestTable") { }

        public int id { get; set; }
        public string columnChar { get; set; }
        public int? columnInt { get; set; }
        public decimal? columnDecimal { get; set; }
        public bool? columnBool { get; set; }
        public DateTime? columnDate { get; set; }
        public DateTime? columnDatetime { get; set; }
        public string columnNotExists { get; set; }
    }

    public class PascalEntity : EntityBase
    {
        public PascalEntity() : base("TestTable") { }

        public int Id { get; set; }
        public string ColumnChar { get; set; }
        public int? ColumnInt { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public bool? ColumnBool { get; set; }
        public DateTime? ColumnDate { get; set; }
        public DateTime? ColumnDatetime { get; set; }
        public string ColumnNotExists { get; set; }
    }

    public class UnderscoreEntity : EntityBase
    {
        public UnderscoreEntity() : base("TestTable") { }

        public int id { get; set; }
        public string column_char { get; set; }
        public int? column_int { get; set; }
        public decimal? column_decimal { get; set; }
        public bool? column_bool { get; set; }
        public DateTime? column_date { get; set; }
        public DateTime? column_datetime { get; set; }
        public string column_not_exists { get; set; }
    }

    public class UpperCaseEntity : EntityBase
    {
        public UpperCaseEntity() : base("TestTable") { }

        public int ID { get; set; }
        public string COLUMNCHAR { get; set; }
        public int? COLUMNINT { get; set; }
        public decimal? COLUMNDECIMAL { get; set; }
        public bool? COLUMNBOOL { get; set; }
        public DateTime? COLUMNDATE { get; set; }
        public DateTime? COLUMNDATETIME { get; set; }
        public string COLUMNNOTEXISTS { get; set; }
    }

    public class UpperCaseSnakeEntity : EntityBase
    {
        public UpperCaseSnakeEntity() : base("TestTable") { }

        public int ID { get; set; }
        public string COLUMN_CHAR { get; set; }
        public int? COLUMN_INT { get; set; }
        public decimal? COLUMN_DECIMAL { get; set; }
        public bool? COLUMN_BOOL { get; set; }
        public DateTime? COLUMN_DATE { get; set; }
        public DateTime? COLUMN_DATETIME { get; set; }
        public string COLUMN_NOT_EXISTS { get; set; }
    }
}
