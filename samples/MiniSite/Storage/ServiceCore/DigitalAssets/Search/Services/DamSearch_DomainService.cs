using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Nwpie.MiniSite.Storage.Common.Services;
using Nwpie.MiniSite.Storage.Common.Utilities;
using Nwpie.MiniSite.Storage.Contract.Assets.Search;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Search.Interfaces;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Search.Services
{
    public class DamSearch_DomainService :
        DomainService,
        IDamSearch_DomainService
    {
        protected void PreProcess<T>(T param)
            where T : DamSearch_Request
        {
            param.Data.OwnerAccountId = param.Data.OwnerAccountId.AssignIfNotSet(GetApplicationAccountId());
        }

        public async Task<DamSearch_Response> Execute(DamSearch_Request param)
        {
            Validate(param);
            PreProcess(param);

            var response = new DamSearch_Response();
            var listRequest = new ResListResource_Request
            {
                Data = param.Data.ConvertTo<ResListResource_RequestModel>()
            };

            var listResponse = await listRequest.InvokeAsyncByBaseUrl<ResListResource_Response>(
                baseUrl: ServiceContext.AuthServiceUrl,
                headers: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
            );

            if (true != listResponse?.Any())
            {
                WrapBadResponseAndThrow(listResponse);
                response.Success(StatusCodeEnum.EmptyData);
                return response;
            }

            response.Data = listResponse.Data;
            await AfterProcess(param, response);

            response.Success();
            return response;
        }

        public async Task AfterProcess(DamSearch_Request param, DamSearch_Response response)
        {
            if (true != response?.Data?.Items?.Count > 0)
            {
                return;
            }

            if (true != param.Data?.Fields?.Contains(nameof(ResListResource_ResponseModelItem.ObjUrl), StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            var s3Client = GetStorage();
            var bucket = CustomConfigUtils.DefaultBucketName;
            var tasks = new List<Task>();
            foreach (var item in response.Data.Items)
            {
                //var s3Result = await s3Client.GetFileUrlAsync(bucket, item.SrcPath);
                //if (s3Result.Any())
                //{
                //    item.ObjUrl = s3Result.Data;
                //}

                tasks.Add(Task.Run(async () =>
                {
                    var s3Result = await s3Client.GetFileUrlAsync(bucket, item.SrcPath);
                    if (s3Result.Any())
                    {
                        item.ObjUrl = s3Result.Data;
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        public bool Validate(DamSearch_Request param)
        {
            if (null == param.Data)
            {
                throw new InvalidOperationException($"Missing data field. ");
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
