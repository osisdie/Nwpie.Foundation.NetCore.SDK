using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Location;
using Nwpie.Foundation.Abstractions.MessageQueue.Models;
using Nwpie.Foundation.Location.Contract.Location.Refresh;
using Nwpie.Foundation.Location.Core;
using Nwpie.Foundation.Location.ServiceCore.Base;
using Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshApiLocation.Interfaces;
using ServiceStack;

namespace Nwpie.Foundation.Location.ServiceCore.Refresh.RefreshApiLocation.Services
{
    public class LocRefreshApiLocation_DomainService :
    DomainService,
    ILocRefreshApiLocation_DomainService
    {
        public async Task<bool> Execute(LocRefresh_Request param)
        {
            XMLReader.Refresh();
            LocationHost.Instance.Refresh();

            _ = BroadCastRefersh(new CommandModel
            {
                Topic = LocationConst.HttpPathToLocationContractRequest_Refresh,
                Name = LocationConst.HttpPathToLocationContractRequest_Refresh,
                Data = new List<string> { "*" }.ToJson()
            });

            await Task.CompletedTask;
            return true;
        }
    }
}
