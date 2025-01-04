using System;
using Nwpie.Foundation.Abstractions.DataAccess.Models;

namespace Nwpie.MiniSite.KVS.Contract.Entities
{
    public abstract class KvsEntity : EntityBase
    {
        public KvsEntity(string tableName) : base(tableName) { }

        public bool? is_del { get; set; }
        public bool? is_hidden { get; set; }
        public string modifier { get; set; }
        public DateTime? updated { get; set; }
        public string creator { get; set; }
        public DateTime? created { get; set; }
    }
}
