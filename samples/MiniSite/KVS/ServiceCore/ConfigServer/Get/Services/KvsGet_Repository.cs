using System.Threading.Tasks;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Services
{
    public class KvsGet_Repository : RepositoryBase, IKvsGet_Repository
    {
        public Task<string> ExecuteQueryVersion(KvsGet_ParamModel param)
        {
            //await Task.CompletedTask;
            //return "5.7.21-log";
            var cmd = new CommandExecutor("Kvs:Get");
            return cmd.ExecuteScalarAsync<string>();
        }
    }
}
