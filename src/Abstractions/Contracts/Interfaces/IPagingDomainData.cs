namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IPagingDomainData : IDomainData
    {
        int? PageIndex { get; set; }
        int? PageSize { get; set; }
    }
}
