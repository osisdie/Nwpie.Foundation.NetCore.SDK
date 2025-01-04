using Nwpie.HostTest.Models;

namespace Nwpie.HostTest.Interfaces
{
    public interface IRepository { }

    public interface IRepository<T> : IRepository
    {
        T FindById(string id);
    }

    public interface IScalarValueRepository : IRepository<string>
    {
    }

    public interface IUserProfileRepository : IRepository<UserProfile>
    {
    }
}
