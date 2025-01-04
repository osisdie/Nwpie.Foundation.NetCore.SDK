using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class ColumnMetaInfo
    {
        public string ColumnName { get; set; }
        public int ColumnOrdinal { get; set; }
        public bool AllowDBNull { get; set; }
        public int MaxLength { get; set; }
        public DbType DbType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
    }
}
