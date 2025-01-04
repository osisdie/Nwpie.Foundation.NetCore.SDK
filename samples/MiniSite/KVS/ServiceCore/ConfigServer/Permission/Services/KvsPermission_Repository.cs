using System.Threading.Tasks;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Permission.Models;

namespace Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.Permission.Services
{
    public class KvsPermission_Repository : RepositoryBase, IKvsPermission_Repository
    {
        public Task<string> ExecuteQueryVersion(KvsPermission_ParamModel param)
        {
            var cmd = new CommandExecutor("Kvs:Permission:Get");
            return cmd.ExecuteScalarAsync<string>();
        }
    }
}
