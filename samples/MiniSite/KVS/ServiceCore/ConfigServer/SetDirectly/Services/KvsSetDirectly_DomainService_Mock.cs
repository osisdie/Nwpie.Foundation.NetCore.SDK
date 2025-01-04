using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.MiniSite.KVS.Common.Services;
using Nwpie.MiniSite.KVS.Contract.Configserver.Set;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Set.Models;
using Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Interfaces;

namespace Nwpie.MiniSite.KVS.ServiceEntry.ConfigServer.SetDirectly.Services
{
    public class KvsSetDirectly_DomainService_Mock :
        DomainService,
        IKvsSetDirectly_DomainService
    {
        public async Task<KvsSet_ResponseModel> Execute(KvsSet_ParamModel param)
        {
            Validate(param);

            await Task.CompletedTask;
            return new KvsSet_ResponseModel
            {
                VersionDisplay = new Version(1, 0, 0).ToString(),
                VersionTimeStamp = DateTime.UtcNow.ToString("s")
            };
        }

        public bool Validate(KvsSet_ParamModel param)
        {
            return base.ValidateAndThrow(param);
        }
    }
}
