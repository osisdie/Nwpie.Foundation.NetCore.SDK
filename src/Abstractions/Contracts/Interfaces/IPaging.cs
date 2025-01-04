namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IPaging
    {
        /// <summary>
        /// Paging index, start from 0 if needed
        /// </summary>
        int? PageIndex { get; set; }

        /// <summary>
        /// Paging size, start from 1 if needed
        /// </summary>
        int? PageSize { get; set; }
    }
}
