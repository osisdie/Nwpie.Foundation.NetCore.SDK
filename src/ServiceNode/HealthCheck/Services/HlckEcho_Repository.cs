using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.HealthCheck.Interfaces;
using Nwpie.Foundation.ServiceNode.HealthCheck.Models;
using Nwpie.Foundation.ServiceNode.ServiceStack.Services;

namespace Nwpie.Foundation.ServiceNode.HealthCheck.Services
{
    public class HlckEcho_Repository :
        RepositoryBase<HlckEcho_Entity>,
        IHlckEcho_Repository
    {
        public async Task<string> ExecuteQueryVersion(HlckEcho_ParamModel param)
        {
            //var cmd = new CommandExecutor("Hlck:Echo:Version");
            //return await cmd.ExecuteScalarAsync<string>();
            await Task.CompletedTask;
            return "8.0.16"; // Expect this version
        }

        public async Task<HlckEcho_Entity> ExecuteQuery(HlckEcho_ParamModel param)
        {
            var item = new HlckEcho_Entity()
            {
                words = param.RequestString
            };

            await Task.CompletedTask;
            return item;
        }
    }
}
