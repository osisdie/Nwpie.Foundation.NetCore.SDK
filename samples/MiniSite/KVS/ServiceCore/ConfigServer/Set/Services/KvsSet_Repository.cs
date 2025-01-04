using System.Threading.Tasks;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Services
{
    public class KvsSet_Repository : RepositoryBase, IKvsSet_Repository
    {
        public Task<string> ExecuteQueryVersion(KvsSet_ParamModel param)
        {
            var cmd = new CommandExecutor("Kvs:Set");
            return cmd.ExecuteScalarAsync<string>();
        }
    }
}
