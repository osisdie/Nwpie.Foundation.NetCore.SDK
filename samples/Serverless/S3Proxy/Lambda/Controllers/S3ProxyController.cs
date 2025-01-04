using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.GetProfile;
using Nwpie.Foundation.S3Proxy.Lambda.Service.Contracts.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.S3Proxy.Lambda.Service.Controllers
{
    /// <summary>
    /// ASP.NET Core controller acting as a S3 Proxy.
    /// </summary>
    //[Route("s3proxy")]
    public class S3ProxyController : Controller
    {
        public S3ProxyController(
            IConfiguration configuration,
            ILogger<S3ProxyController> logger,
            IAmazonS3 s3Client,
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache)
        {
            Logger = logger;
            m_S3Client = s3Client;
            m_MemoryCache = memoryCache;
            m_HttpClientFactory = httpClientFactory;

            var baseUrl = configuration[Startup.AppAuthUrlKey];
            if (string.IsNullOrEmpty(baseUrl))
            {
                logger.LogCritical("Missing configuration for auth url. The AppAuthUrl configuration must be set to a http base url.");
                throw new Exception("Missing configuration for auth url. The AppAuthUrl configuration must be set to a http base url.");
            }

            m_AuthUrl = $"{baseUrl}/Account/AcctGetProfileRequest";
        }

        [HttpGet]
        [Route("s3proxy/index")]
        public string Index()
        {
            return DateTime.UtcNow.ToString("s");
        }

        [HttpPost]
        [HttpPut]
        [Route("s3proxy/upload")]
        [Produces("application/json")]
        public async Task<ImageUpload_Response> Upload(ImageUpload_Request request)
        {
            var response = new ImageUpload_Response();
            if (true != Validate(request, response))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }

            //if (true != await Authentication(request, response))
            //{
            //    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    return response;
            //}

            var file = Request.Form.Files.First();
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = request.Bucket,
                        Key = request.FileKey,
                        InputStream = stream
                    };

                    var uploaded = await m_S3Client.PutObjectAsync(putRequest);
                    if (false == string.IsNullOrWhiteSpace(uploaded?.ResponseMetadata?.RequestId))
                    {
                        response.IsSuccess = true;
                        response.Code = 0001; // General success
                    }
                    else
                    {
                        response.Code = 107; // EMErrorCode_Platform.Data_Empty
                    }

                    response.Msg = $"Uploaded object {request.FileKey} to bucket {request.Bucket}. Request Id: {uploaded?.ResponseMetadata?.RequestId}";
                    Logger.LogInformation(response.Msg);
                }
            }
            catch (AmazonS3Exception e)
            {
                Response.StatusCode = (int)e.StatusCode;
                response.ErrMsg = e.Message;
                response.Code = 0003; // General error.exception
                Logger.LogError(e.ToString());
                return response;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrMsg = e.Message;
                response.Code = 0003; // General error.exception
                Logger.LogError(e.ToString());
                return response;
            }

            Response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }

        public bool Validate(ImageUpload_Request param, ImageUpload_Response response)
        {
            try
            {
                if (1 != Request?.Form?.Files?.Count)
                {
                    throw new ArgumentException($"Require only 1 file stream, but received {Request.Form?.Files?.Count()} files");
                }

                if (true != Request?.Form?.Files?.First()?.Length > 0)
                {
                    throw new ArgumentException($"Received empty file");
                }

                if (string.IsNullOrWhiteSpace(param?.Bucket))
                {
                    throw new ArgumentNullException($"Require {nameof(param.Bucket)} field");
                }

                if (string.IsNullOrWhiteSpace(param?.FileKey))
                {
                    throw new ArgumentNullException($"Require {nameof(param.FileKey)} field");
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                response.Code = 10; // EMErrorCode_Platform.ContractRequest_ValidateFail
                response.ErrMsg = ex.Message;
            }

            return false;
        }

        public async Task<bool> Authentication(ImageUpload_Request param, ImageUpload_Response response)
        {
            try
            {
                string authToken = Request?.Headers?["Authorization"];
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    response.ErrMsg = nameof(UnauthorizedAccessException);
                    throw new UnauthorizedAccessException();
                }

                if (true != m_MemoryCache?.TryGetValue(authToken, out _))
                {
                    // Ask Todo
                    using (var httpClient = m_HttpClientFactory.CreateClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", authToken);

                        var requestDto = new GetProfile_Request();
                        var result = await httpClient.PostAsync(m_AuthUrl,
                            requestDto,
                            new JsonMediaTypeFormatter()
                        );

                        if (true != result?.IsSuccessStatusCode)
                        {
                            response.ErrMsg = result?.StatusCode.ToString()
                                ?? HttpStatusCode.InternalServerError.ToString();
                            throw new Exception(response.ErrMsg);
                        }

                        var responseDto = await result.Content.ReadAsAsync<GetProfile_Pesponse>();
                        if (true != responseDto?.IsSuccess || null == responseDto?.Data?.AccountId)
                        {
                            response.ErrMsg = responseDto?.ErrMsg
                                ?? responseDto?.Msg
                                ?? nameof(UnauthorizedAccessException);
                            throw new UnauthorizedAccessException();
                        }

                        m_MemoryCache?.Set(authToken,
                            responseDto.Data.AccountId,
                            new TimeSpan(0, 15, 0)
                        ); // 15 minutes
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                response.Code = 41; // Invalid AccessToken
            }

            return false;
        }

        private readonly ILogger Logger;
        private readonly IMemoryCache m_MemoryCache;
        private readonly IHttpClientFactory m_HttpClientFactory;
        private readonly IAmazonS3 m_S3Client;

        private readonly string m_AuthUrl;
    }
}
