using System.Threading.Tasks;

namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IDataContext
    {
        void AddCommand(ICommand command);
        Task CommitAsync();
    }
}
