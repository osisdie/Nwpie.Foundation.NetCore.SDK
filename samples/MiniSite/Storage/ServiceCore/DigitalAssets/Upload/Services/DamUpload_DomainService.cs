using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Auth.Contract.Resource.AddResource;
using Nwpie.Foundation.Auth.Contract.Resource.ListResource;
using Nwpie.Foundation.Auth.Contract.Resource.UptResource;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using Nwpie.MiniSite.Storage.Common.Domain;
using Nwpie.MiniSite.Storage.Common.Services;
using Nwpie.MiniSite.Storage.Common.Utilities;
using Nwpie.MiniSite.Storage.Contract.Assets.Exists;
using Nwpie.MiniSite.Storage.Contract.Assets.Upload;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Exists.Interfaces;
using Nwpie.MiniSite.Storage.ServiceCore.Assets.Upload.Interfaces;
using Microsoft.AspNetCore.Http;
using ServiceStack;

namespace Nwpie.MiniSite.Storage.ServiceCore.Assets.Upload.Services
{
    public class DamUpload_DomainService :
        DomainService,
        IDamUpload_DomainService
    {
        public async Task<DamUpload_Response> Execute(DamUpload_Request param)
        {
            if (true != CurrentRequest.Form?.Files?.Count > 0)
            {
                throw new InvalidOperationException("File Stream not found. ");
            }

            if (1 != CurrentRequest.Form?.Files?.Count)
            {
                throw new InvalidOperationException("Too many files in Stream. ");
            }

            return await Execute(param, CurrentRequest.Form.Files.First());
        }

        public async Task<DamUpload_Response> Execute(DamUpload_Request param, IFormFile file)
        {
            Validate(param);

            // - gltf (parent)
            //   - texture (parent: gltf)
            //   - model (parent: gltf)
            var response = new DamUpload_Response();
            IServiceResponse<bool> s3Result = null;
            using (var fileStram = file.OpenReadStream())
            {
                s3Result = await GetStorage().UploadAsync(param.Bucket ?? CustomConfigUtils.DefaultBucketName, param.Key, fileStram);
            }

            if (false == s3Result?.IsSuccess)
            {
                SubCode = s3Result.Code > 0 ? ((StatusCodeEnum)s3Result.Code).ToString() : "";
                SubMsg = s3Result.ErrMsg ?? s3Result.Msg;
                throw new Exception(SubMsg);
            }

            if (s3Result.Any())
            {
                var existSvc = GetDomainService<IDamExists_DomainService>();
                var existResult = await existSvc.Execute(new DamExists_Request
                {
                    Bucket = param.Bucket,
                    Key = param.Key
                });

                if (existResult.Any())
                {
                    response.Data = await UpdateResource(existResult.Data.ObjId, param);
                }
                else
                {
                    response.Data = await CreateResource(param);
                }

                response.Success();
                return response;
            }

            return null;
        }

        protected async Task<ResListResource_ResponseModelItem> CreateResource(DamUpload_Request param)
        {
            var request = new ResAddResource_Request
            {
                Data = new ResAddResource_RequestModel
                {
                    OwnerAccountId = GetApplicationAccountId(),
                    ParentObjId = param.ParentObjId,
                    PermId = LookupHelper.GetPermissionId(PermissionLevelEnum.Full),
                    DisplayName = param.FileName ?? Path.GetFileName(param.Key),
                    IsFunc = false,
                    SrcId = param.Key.ToMD5(),
                    SrcPath = param.Key,
                    Description = param.Description,
                    Tags = param.Tags,
                    Metadata = param.Metadata,
                }
            };

            var response = await request.InvokeAsyncByBaseUrl<ResAddResource_Response>(
                baseUrl: ServiceContext.AuthServiceUrl,
                headers: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
            );

            if (response.Any())
            {
                return new ResListResource_ResponseModelItem()
                {
                    ObjId = response.Data.ObjId
                };
            }

            return null;
        }

        protected async Task<ResListResource_ResponseModelItem> UpdateResource(string objId, DamUpload_Request param)
        {
            var request = new ResUptResource_Request
            {
                Data = new ResUptResource_RequestModel
                {
                    OwnerAccountId = GetApplicationAccountId(),
                    ObjId = objId,
                    PermId = LookupHelper.GetPermissionId(PermissionLevelEnum.Full),
                    DisplayName = param.FileName ?? Path.GetFileName(param.Key),
                    //SrcId = $"{param.Bucket}:{param.FileKey}".ToMD5(),
                    //SrcPath = $"{param.Bucket}:{param.FileKey}",
                    Description = param.Description,
                    Tags = param.Tags,
                    Metadata = param.Metadata,
                }
            };

            var response = await request.InvokeAsyncByBaseUrl<ResUptResource_Response>(
                baseUrl: ServiceContext.AuthServiceUrl,
                headers: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).AddApiKeyHeader()
            );

            if (response.Any())
            {
                return new ResListResource_ResponseModelItem()
                {
                    ObjId = objId
                };
            }

            return null;
        }

        public bool Validate(DamUpload_Request param)
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

            if (null == GetRequester())
            {
                throw new InvalidOperationException("Invalid requester. ");
            }

            return base.ValidateAndThrow(param);
        }
    }
}
