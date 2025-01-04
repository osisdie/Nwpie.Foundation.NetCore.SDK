namespace Nwpie.Foundation.Abstractions.Contracts.Interfaces
{
    public interface IDomainData
    {
        bool Validate();
        bool ValidateAndThrow();
    }
}
