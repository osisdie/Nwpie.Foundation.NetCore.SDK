﻿using System.Threading.Tasks;
using Nwpie.Foundation.ServiceNode.ServiceStack.Interfaces;
using Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Models;

namespace Nwpie.MiniSite.KVS.ServiceCore.ConfigServer.Get.Interfaces
{
    public interface IKvsGet_Repository : IDomainRepository
    {
        Task<string> ExecuteQueryVersion(KvsGet_ParamModel param);
    }
}
