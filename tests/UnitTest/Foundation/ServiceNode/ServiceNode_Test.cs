using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Auth.Contract.Extensions;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Configuration.SDK.Contracts.Get;
using Nwpie.Foundation.Http.Common.Utilities;
using Nwpie.Foundation.ServiceNode.SDK.Extensions;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class ServiceNode_Test : TestBase
    {
        public ServiceNode_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Route_Test()
        {
            var attr = typeof(EchoTest_Request)
                .GetCustomAttributes<RouteAttribute>()
                ?.FirstOrDefault();
            Assert.NotNull(attr);
            Assert.Equal("/HealthCheck/Echo", attr.Path);

            // standard endpoint config
            {
                var endpoint = "http://localhost:5000";
                var absoleteUrl = string.Concat(
                    endpoint,
                    attr.Path
                );
                Assert.Equal("http://localhost:5000/HealthCheck/Echo", absoleteUrl);
            }

            // safe concate url path
            {
                var endpoint = @"http://localhost:5000/";
                var absoleteUrl = $"{endpoint.TrimEndSlash()}/{attr.Path.TrimStartSlash()}";
                Assert.Equal("http://localhost:5000/HealthCheck/Echo", absoleteUrl);
            }

            // remove redundant end slash
            {
                var endpoint = "http://localhost:5000/";
                var absoleteUrl = string.Concat(
                    endpoint.TrimEndSlash(),
                    attr.Path
                );
                Assert.Equal("http://localhost:5000/HealthCheck/Echo", absoleteUrl);
            }

            // remove redundant backslash
            {
                var endpoint = @"http://localhost:5000\";
                var absoleteUrl = string.Concat(
                   endpoint.TrimEndSlash(),
                   attr.Path
               );
                Assert.Equal("http://localhost:5000/HealthCheck/Echo", absoleteUrl);
            }
        }

        [Fact]
        public void Url_Test()
        {
            var request = new EchoTest_Request()
            {
                Data = new EchoTest_RequestModel()
                {
                    RequestString = "ok"
                }
            };

            var url = "http://api-dev.kevinw.net/todo" + request.ToPostUrl();
            Assert.Equal("http://api-dev.kevinw.net/todo/HealthCheck/Echo", url);
        }

        [Fact]
        public void OK_Test()
        {
            {
                var resp = new ContractResponseBase<ResultDtoBase>();
                Assert.False(resp.IsSuccess);
            }

            {
                var resp = new ContractResponseBase<ResultDtoBase>()
                    .Success();
                Assert.True(resp.IsSuccess);
            }

            {
                var resp = new ContractResponseBase<ResultDtoBase>();
                resp.Success();
                Assert.True(resp.IsSuccess);
            }

            {
                var resp = new ContractResponseBase<ResultDtoBase>();
                resp.Success(StatusCodeEnum.Success);
                Assert.True(resp.IsSuccess);
            }
        }

        [Fact]
        public void Fail_Test()
        {
            {
                var resp = new ContractResponseBase<ResultDtoBase>();
                Assert.False(resp.IsSuccess);
            }

            {
                var resp = new ContractResponseBase<ResultDtoBase>()
                    .Error();
                Assert.False(resp.IsSuccess);
            }

            {
                var resp = new ContractResponseBase<ResultDtoBase>()
                    .Error();
                Assert.False(resp.IsSuccess);
            }

            {
                var json = "{}";
                var dto = Serializer.Deserialize<ContractResponseBase<ResultDtoBase>>(json);
                Assert.False(dto.IsSuccess);
            }
        }

        [Fact]
        public void Empty_Test()
        {
            {
                var resp = new ContractResponseBase<ResultDtoBase>();
                Assert.False(resp.IsSuccess);
            }

            {
                var resp = new ContractResponseBase<ResultDtoBase>();
                resp.Success(StatusCodeEnum.EmptyData);

                Assert.True(resp.IsSuccess);
                Assert.Equal((int)StatusCodeEnum.EmptyData, resp.Code);
            }
        }

        [Fact]
        public async Task Echo_Test()
        {
            // Correct Request
            {
                var request = new EchoTest_Request()
                {
                    Data = new EchoTest_RequestModel()
                    {
                        RequestString = "123"
                    }
                };

                using (var service = new EchoTest_Service())
                {
                    var response = await service.Any(request);
                    Assert.True(response.IsSuccess);
                    Assert.Equal((int)StatusCodeEnum.Success, response.Code);
                    Assert.NotNull(response.Data);
                    Assert.Equal(request.Data.RequestString, response.Data.ResponseString);
                }
            }

            // Wrong request: null request
            {
                var service = new EchoTest_Service();
                try
                {
                    using (service)
                    {
                        var response = await service.Any(null);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // Wrong request: missing Arguments
            {
                var request = new EchoTest_Request();
                var service = new EchoTest_Service();
                service.InitEvent += () =>
                {
                    service.ThrowIfInvalid = false;
                };

                try
                {
                    using (service)
                    {
                        var response = await service.Any(request);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // Wrong request: null Arguments
            {
                var request = new EchoTest_Request
                {
                    Data = new EchoTest_RequestModel
                    {
                        RequestString = null
                    }
                };

                var service = new EchoTest_Service
                {
                    ThrowIfInvalid = true
                };

                try
                {
                    using (service)
                    {
                        var response = await service.Any(request);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }
        }

        [Fact]
        public async Task SimpleEcho_Test()
        {
            // Correct Request
            {
                var request = new SimpleEchoTest_Request()
                {
                    Data = new SimpleEchoTest_RequestModel()
                    {
                        RequestString = "123"
                    }
                };

                using (var service = new SimpleEchoTest_Service())
                {
                    var responseString = await service.Any(request);
                    Assert.Equal(request.Data.RequestString, responseString);
                }
            }

            // Wrong request: null request
            {
                var service = new SimpleEchoTest_Service();
                try
                {
                    using (service)
                    {
                        var response = await service.Any(null);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // Wrong request: missing Arguments
            {
                var request = new SimpleEchoTest_Request();
                var service = new SimpleEchoTest_Service();
                service.InitEvent += () =>
                {
                    service.ThrowIfInvalid = false;
                };

                try
                {
                    using (service)
                    {
                        var response = await service.Any(request);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // Wrong request: null Arguments
            {
                var request = new SimpleEchoTest_Request
                {
                    Data = new SimpleEchoTest_RequestModel
                    {
                        RequestString = null
                    }
                };

                var service = new SimpleEchoTest_Service
                {
                    ThrowIfInvalid = true
                };

                try
                {
                    using (service)
                    {
                        var response = await service.Any(request);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }
        }

        [Fact]
        public async Task SimpleStringEcho_Test()
        {
            // Correct Request
            {
                var request = new SimpleStringEchoTest_Request()
                {
                    RequestString = "123"
                };

                using (var service = new SimpleStringEchoTest_Service())
                {
                    var responseString = await service.Any(request);
                    Assert.Equal(request.RequestString, responseString);
                }
            }

            // Wrong request: null request
            {
                var service = new SimpleStringEchoTest_Service();
                try
                {
                    using (service)
                    {
                        var response = await service.Any(null);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // Wrong request: missing Arguments
            {
                var request = new SimpleStringEchoTest_Request();
                var service = new SimpleStringEchoTest_Service();
                service.InitEvent += () =>
                {
                    service.ThrowIfInvalid = false;
                };

                try
                {
                    using (service)
                    {
                        var response = await service.Any(request);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }

            // Wrong request: null Arguments
            {
                var request = new SimpleStringEchoTest_Request
                {
                    RequestString = null
                };

                var service = new SimpleStringEchoTest_Service
                {
                    ThrowIfInvalid = true
                };

                try
                {
                    using (service)
                    {
                        var response = await service.Any(request);
                    }
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }
            }
        }

        [Fact(Skip = "Won't test remote config service")]
        public async Task POST_ReturnNotNull()
        {
            var request = new ConfigGet_Request()
            {
                Data = new ConfigGet_RequestModel()
                {
                    ConfigKeys = new List<ConfigGet_RequestModelItem>
                    {
                        new ConfigGet_RequestModelItem()
                        {
                            ConfigKey = SysConfigKey.Default_Notification_HostUrl_ConfigKey
                        }
                    }
                }
            };

            var absoleteUrl = string.Concat(
                ServiceContext.ConfigServiceUrl.TrimEndSlash(),
                request.ToUrl(ServiceClientBase.DefaultHttpMethod)
            );

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                .AddApiKeyHeader();
            Assert.Equal("https://api-dev.kevinw.net/config/get",
                absoleteUrl);

            ConfigGet_Response response;

            // crash
            //{
            //    using (var client = new JsonServiceClient())
            //    {
            //        response = await client
            //            .AddHeaders(headers)
            //            .PostAsync<AppListApiKey_Response>(absoleteUrl, request);
            //    }

            //    Assert.True(response?.IsSuccess);
            //    Assert.NotEmpty(response.Data?.Items);
            //}

            {
                var result = await ApiUtils.PostWebApi<ConfigGet_Response>(url: absoleteUrl,
                    jsonData: request.ToJson(),
                    headers: headers
                );
                response = result?.Data;
                Assert.True(response?.IsSuccess);
                Assert.NotEmpty(response.Data?.RawData);
            }

            {
                response = await request.InvokeAsyncByAbsoleteUrl<ConfigGet_Response>(
                    url: absoleteUrl,
                    headers: headers
                );
                Assert.True(response?.IsSuccess);
                Assert.NotEmpty(response.Data?.RawData);
            }

            {
                response = await request.InvokeAsyncByBaseUrl<ConfigGet_Response>(
                    baseUrl: ServiceContext.ConfigServiceUrl,
                    headers: headers
                );
                Assert.True(response?.IsSuccess);
                Assert.NotEmpty(response.Data?.RawData);
            }
        }

        public override Task<bool> IsReady()
        {
            var env = Utility.GetSDKEnvNameByApiName("todo.dev");
            var authConfigValue = GetMapValueDefault(
                SysConfigKey.Default_Auth_ConfigKey,
                env,
                EnvironmentEnum.Max
            );
            Assert.NotNull(authConfigValue);

            m_AuthOption = new ConfigOptions<Auth_Option>(
                Serializer.Deserialize<Auth_Option>(authConfigValue)
            );
            Assert.NotNull(m_AuthOption?.Value);
            Assert.NotNull(m_AuthOption.Value.AdminToken);
            Assert.NotNull(ServiceContext.AuthServiceUrl);

            return base.IsReady();
        }

        protected IConfigOptions<Auth_Option> m_AuthOption;
    }
}
