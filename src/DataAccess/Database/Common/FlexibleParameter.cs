using System;
using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class FlexibleParameter
    {
        public DbType GetDbType() => m_DbType;
        public void SetDbType(DbType value) => m_DbType = value;

        public string Name { get; set; }
        public object Value { get; set; }
        public int Size { get; set; }
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool IsDiy { get; set; }
        public bool Enable { get; set; }
        public Type Type { get; set; }
        public bool IsPrimaryKey { get; set; }

        protected DbType m_DbType;
    }
}
