using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Nwpie.MiniSite.Storage.Common.Services;
using Nwpie.MiniSite.Storage.Common.Utilities;
using Nwpie.MiniSite.Storage.Contract.Assets.Exists;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Exists.Interfaces;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Exists.Services
{
    public class DamExists_DomainService :
        DomainService,
        IDamExists_DomainService
    {
        public async Task<DamExists_Response> Execute(DamExists_Request param)
        {
            Validate(param);

            var response = new DamExists_Response();

            var exists = await GetStorage().IsFileExistAsync(param.Bucket ?? CustomConfigUtils.DefaultBucketName, $"{param.Key}");
            if (true == exists?.Data)
            {
                var getRequest = new ResListResource_Request
                {
                    Data = new ResListResource_RequestModel
                    {
                        OwnerAccountId = GetApplicationAccountId(),
                        SrcIds = new List<string>() { $"{param.Key}".ToMD5() },
                        Hierarchy = AuthServiceConfig.MaxHierarchy,
                        PageIndex = ConfigConst.MinPageIndex,
                        PageSize = ConfigConst.MinPageSize,
                    }
                };

                var getResponse = await getRequest.InvokeAsyncByBaseUrl<ResListResource_Response>(
                    baseUrl: ServiceContext.AuthServiceUrl,
                    headers: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
                );

                if (true != getResponse?.Any())
                {
                    WrapBadResponseAndThrow(getResponse);
                    response.Success(StatusCodeEnum.EmptyData, "File exists but asset not exists. Please upload again. ");
                    return response;
                }

                response.Data = getResponse.Data.Items.First().ConvertTo<ResListResource_ResponseModelItem>();
                response.Success();
                return response;
            }

            response.Success(StatusCodeEnum.EmptyData);
            return response;
        }

        public bool Validate(DamExists_Request param)
        {
            if (true == param?.Bucket?.Contains("/") || true == param?.Bucket?.Contains("\\"))
            {
                throw new InvalidOperationException($"Invalid bucket name. Bucket: {param?.Bucket}. ");
            }

            if (true == param?.Key?.StartsWith("/") || true == param?.Key?.StartsWith("\\"))
            {
                throw new InvalidOperationException($"Invalid fileKey name. FileKey: {param?.Key}. ");
            }

            if (null == GetStorage())
            {
                throw new InvalidOperationException("Storage service is not ready. ");
            }

            if (null == GetApplicationAccountId())
            {
                throw new InvalidOperationException("Storage service account is not ready. ");
            }

            return base.ValidateAndThrow(param);
        }
    }
}
