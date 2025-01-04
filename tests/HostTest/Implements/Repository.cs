using System;
using Nwpie.HostTest.Interfaces;
using Nwpie.HostTest.Models;

namespace Nwpie.HostTest.Implements
{
    public class ScalarStringRepository : IScalarValueRepository
    {
        public string FindById(string id)
        {
            throw new NotImplementedException();
        }
    }
    public class UserProfileRepository : IUserProfileRepository
    {
        public UserProfile FindById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
