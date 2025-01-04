using Nwpie.Foundation.Abstractions.DataAccess.Models;

namespace Nwpie.Foundation.Abstractions.DataAccess.Entities
{
    public class DBVersion_Entity : EntityBase
    {
        public DBVersion_Entity() : base("") { }

        public string version { get; set; }
    }
}
