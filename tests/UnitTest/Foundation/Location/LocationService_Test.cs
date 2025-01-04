using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.ApiKey.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Location.Interfaces;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Auth.Contract;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Location.Contract.Location.GetLocation;
using Nwpie.Foundation.Location.Core;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Location
{
    public class LocationService_Test : TestBase
    {
        public LocationService_Test(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Regex_ReplaceEnv_ReturnApiNameString()
        {
            var pattern = AuthServiceConfig.MatchApiNamePattern;

            {
                var setEnv = EnvironmentEnum.Development.GetDisplayName();
                var apiName = $"{DeploymentAppName}.{setEnv}";
                Assert.Matches(pattern, apiName);

                var apiWithoutEnv = Utility.GetSDKAppNameByApiName(apiName);
                var env = Utility.GetSDKEnvNameByApiName(apiName);
                Assert.Equal(DeploymentAppName, apiWithoutEnv);
                Assert.Equal(setEnv, env);
            }

            {
                var setEnv = EnvironmentEnum.Production.GetDisplayName();
                var apiName = $"{DeploymentAppName}dev.{setEnv}";
                Assert.Matches(pattern, apiName);

                var apiWithoutEnv = Utility.GetSDKAppNameByApiName(apiName);
                var env = Utility.GetSDKEnvNameByApiName(apiName);
                Assert.Equal($"{DeploymentAppName}dev", apiWithoutEnv);
                Assert.Equal(setEnv, env);
            }

            {
                var setEnv = EnvironmentEnum.Staging.GetDisplayName();
                var apiName = $"{DeploymentAppName}dev{setEnv}";
                Assert.Matches(pattern, apiName);

                var apiWithoutEnv = Utility.GetSDKAppNameByApiName(apiName);
                var env = Utility.GetSDKEnvNameByApiName(apiName);
                Assert.Equal($"{DeploymentAppName}devstage", apiWithoutEnv);
            }
        }

        [Fact(Skip = "Won't test remote location service")]
        public void GetApiLocation_ViaHost_ReturnNotNull()
        {
            var request = new LocGetApiLocation_Request()
            {
                ApiKey = ServiceContext.ApiKey,
                AppName = DeploymentAppName,
                EnvInfo = new EnvInfo()
                {
                    Env = ServiceContext.SdkEnv,
                    IP = NetworkUtils.IP
                }
            };

            var batchRequest = new LocBatchGetApiLocations_Request()
            {
                ApiKey = ServiceContext.ApiKey,
                AppNames = new List<string> { DeploymentAppName },
                EnvInfo = new EnvInfo()
                {
                    Env = ServiceContext.SdkEnv,
                    IP = NetworkUtils.IP
                }
            };

            {
                request.EnvInfo.Env = EnvironmentEnum.Debug.GetDisplayName();

                var response = LocationHost.Instance.GetApiLocation(request);
                Assert.NotNull(response);
                Assert.NotNull(response.Data);
                Assert.Equal($"{ApiDevelopmentBaseUrl}/{DeploymentAppName}", response.Data);

                batchRequest.EnvInfo.Env = EnvironmentEnum.Debug.GetDisplayName();
                var batchResponse = LocationHost.Instance.BatchGetApiLocations(batchRequest);
                Assert.NotNull(batchResponse);
                Assert.NotEmpty(batchResponse.Data);
                Assert.Equal(DeploymentAppName, batchResponse.Data.First().Key);
                Assert.Equal($"{ApiDevelopmentBaseUrl}/{DeploymentAppName}", batchResponse.Data.First().Value);
            }

            {
                request.EnvInfo.Env = EnvironmentEnum.Production.GetDisplayName();

                var response = LocationHost.Instance.GetApiLocation(request);
                Assert.NotNull(response);
                Assert.NotNull(response.Data);
                Assert.Equal($"{ApiProductionBaseUrl}/{DeploymentAppName}", response.Data);

                batchRequest.EnvInfo.Env = EnvironmentEnum.Production.GetDisplayName();
                var batchResponse = LocationHost.Instance.BatchGetApiLocations(batchRequest);
                Assert.NotNull(batchResponse);
                Assert.NotEmpty(batchResponse.Data);
                Assert.Equal(DeploymentAppName, batchResponse.Data.First().Key);
                Assert.Equal($"{ApiProductionBaseUrl}/{DeploymentAppName}", batchResponse.Data.First().Value);
            }
        }

        [Fact(Skip = "Won't test remote location service")]
        public async Task GetApiLocation_ViaSDKHelper_ReturnNotNull()
        {
            var response = await m_LocationClient?.GetApiLocation(DeploymentAppName);
            Assert.NotNull(response);
            Assert.Equal($"{ApiDevelopmentBaseUrl}/{DeploymentAppName}", response);
        }

        public override Task<bool> IsReady()
        {
            m_LocationClient = ComponentMgr.Instance.TryResolve<ILocationClient>();
            Assert.NotNull(m_LocationClient);

            return base.IsReady();
        }

        private const string DeploymentAppName = "todo";
        private const string ApiDevelopmentBaseUrl = "https://api-dev.kevinw.net";
        private const string ApiProductionBaseUrl = "https://api.kevinw.net";

        protected ILocationClient m_LocationClient;
    }
}
