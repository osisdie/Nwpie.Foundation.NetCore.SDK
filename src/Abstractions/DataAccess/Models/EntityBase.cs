using System;
using Nwpie.Foundation.Abstractions.DataAccess.Interfaces;

namespace Nwpie.Foundation.Abstractions.DataAccess.Models
{
    public abstract class EntityBase : IEntity
    {
        public EntityBase(string tableName)
        {
            TableName = tableName;
        }

        public virtual string TableName { get; set; }

        public virtual string message { get; set; }
        public virtual int? return_value { get; set; }
        public DateTime? _ts { get; set; }
    }
}
