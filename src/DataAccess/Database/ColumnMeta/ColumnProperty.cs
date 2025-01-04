using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class ColumnProperty : IKeyedObject, IKeyedObject<string>
    {
        public string Key
        {
            get => ColumnName;
        }

        public string ColumnName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public DbType DbType { get; set; }
        public int Size { get; set; }
        public bool IsNullable { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
    }
}
