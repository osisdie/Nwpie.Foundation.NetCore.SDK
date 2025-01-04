using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract.Enums.Status;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Auth.SDK.Utilities
{
    public sealed class ResourceHelper
    {
        static ResourceHelper()
        {
            Logger = LogMgr.CreateLogger(typeof(ResourceHelper));
        }

        public static async Task<ResListResource_ResponseModelItem> GetFirstResourceAsync(ResListResource_Request request, string hostUrl = null)
        {
            request.Data.PageIndex = ConfigConst.MinPageIndex;
            request.Data.PageSize = ConfigConst.MinPageSize;

            return (await GetResourceAsync(request, hostUrl))?.FirstOrDefault();
        }

        public static async Task<IEnumerable<ResListResource_ResponseModelItem>> GetResourceAsync(ResListResource_Request request, string hostUrl = null)
        {
            try
            {
                var response = await request.InvokeAsyncByBaseUrl<ResListResource_Response>(
                    baseUrl: hostUrl ?? ServiceContext.AuthServiceUrl,
                    headers: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
                );

                return response?.Data?.Items;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            return null;
        }

        #region status
        public static bool IsActive(string status) =>
            status == ResourceStatusEnum.Active.GetDisplayName();

        public static bool IsNotActive(string status) =>
            status == ResourceStatusEnum.InActive.GetDisplayName();

        #endregion

        #region converter
        public static string ConvertTo_Active_StatusCode() =>
            ResourceStatusEnum.Active.GetDisplayName();

        public static string ConvertTo_InActive_StatusCode() =>
            ResourceStatusEnum.InActive.GetDisplayName();

        #endregion

        private static readonly ILogger Logger;
        //private static readonly ICache m_Cache; // TODO: use cache
    }
}
