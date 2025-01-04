using System.Collections.Generic;
using System.Linq;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.MiniSite.KVS.Contract.Enums;

namespace Nwpie.MiniSite.KVS.Contract.Configserver.Permission
{
    public class KvsPermission_RequestModelItem
    {
        public string Creator { get; set; }

        public AccessLevelEnum AllowRead_Behavior { get; set; }
        public List<string> AllowRead_ApiNames { get; set; }

        public AccessLevelEnum AllowWrite_Behavior { get; set; }
        public List<string> AllowWrite_ApiNames { get; set; }
    }

    public static class KvsPermission_ResponseModelItem_Extension
    {
        public static void AddToWiteList(this KvsPermission_RequestModelItem o, KvsBehaviorEnum behavior, string apiName)
        {
            switch (behavior)
            {
                case KvsBehaviorEnum.Both:
                    if (false == o.AllowRead_ApiNames.Contains(apiName))
                    {
                        o.AllowRead_ApiNames.Add(apiName);
                    }

                    if (false == o.AllowWrite_ApiNames.Contains(apiName))
                    {
                        o.AllowWrite_ApiNames.Add(apiName);
                    }
                    break;
                case KvsBehaviorEnum.Read:
                    if (false == o.AllowRead_ApiNames.Contains(apiName))
                    {
                        o.AllowRead_ApiNames.Add(apiName);
                    }
                    break;
                case KvsBehaviorEnum.Write:
                    if (false == o.AllowWrite_ApiNames.Contains(apiName))
                    {
                        o.AllowWrite_ApiNames.Add(apiName);
                    }
                    break;
            };
        }

        public static void AddToWiteList(this KvsPermission_RequestModelItem o, KvsBehaviorEnum behavior, List<string> apiNames)
        {
            if (apiNames?.Count() > 0)
            {
                for (var iApiName = 0; iApiName < apiNames.Count(); ++iApiName)
                {
                    o.AddToWiteList(behavior, apiNames[iApiName]);
                }
            }
        }

    }

}
