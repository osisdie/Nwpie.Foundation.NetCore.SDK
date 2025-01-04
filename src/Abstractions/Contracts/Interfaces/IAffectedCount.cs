namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IAffectedCount
    {
        /// <summary>
        /// Count Inserted to DB
        /// </summary>
        int CountInserted { get; set; }

        /// <summary>
        /// Count Inserted to DB
        /// </summary>
        int CountUpdated { get; set; }

        /// <summary>
        /// Count incomplete jobs
        /// </summary>
        int CountFailed { get; set; }

        /// <summary>
        /// Count Deleted from DB
        /// </summary>
        int CountDeleted { get; set; }
    }
}
