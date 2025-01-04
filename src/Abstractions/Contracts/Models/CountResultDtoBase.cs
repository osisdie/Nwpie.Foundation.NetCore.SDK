using Nwpie.Foundation.Abstractions.Contracts.Interfaces;

namespace Nwpie.Foundation.Abstractions.Contracts.Models
{
    public class CountResultDtoBase : ResultDtoBase, IAffectedCount
    {
        /// <summary>
        /// Count Inserted to DB
        /// </summary>
        public int CountInserted { get; set; }

        /// <summary>
        /// Count Inserted to DB
        /// </summary>
        public int CountUpdated { get; set; }

        /// <summary>
        /// Count incomplete jobs
        /// </summary>
        public int CountFailed { get; set; }

        /// <summary>
        /// Count Deleted from DB
        /// </summary>
        public int CountDeleted { get; set; }
    }
}
