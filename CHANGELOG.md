### 2025-01-03
* **Nwpie.Foundation.***
  * pkge: Upgrade `Microsoft` packages to 9.0.0 *
* **Nwpie.xUnit**
  * chore: Fix failed tests and suppress SQL tests (TODO)

### 2020-09-18
* **Nwpie.Foundation.Abstractions & Nwpie.Foundation.Common (2.0.2)**
  * pkge: Upgrade `Microsoft` package 3.1.7 -> 3.1.8
* **Nwpie.Foundation.Logging (1.1.1)**
  * pkge: Upgrade `Serilog.Sinks.ElasticSearch` package 8.2.0 -> 8.4.0
  * pkge: Upgrade `NLog` package 4.7.3 -> 4.7.4
  * pkge: Upgrade `Log4net` package 2.0.8 -> 2.0.10
* **Nwpie.Foundation.Storage.S3 (1.9.2)**
  * pkge: Upgrade `AWSSDK.S3` package 3.3.* -> 3.5.*
* **All the other modules (1.9.2)**
  * pkge: Upgrade version from 1.9.1.* -> 1.9.2.*
* **All Endpoint**
  * pkge: Reference `AWSSDK.Core` package 3.5.*
* **All Serverless**
  * pkge: Reference `AWSSDK.Core` package 3.5.*
  * pkge: Upgrade `Amazon.Lambda.AspNetCoreServer` package 5.1.3 -> 5.1.5

### 2020-08-17
* **Nwpie.Foundation.Abstractions (2.0.1)**
  * pkge: Upgrade `Microsoft` package 3.1.6 -> 3.1.7
  * pkge: Upgrade `ServiceStack` package 5.9.0 -> 5.9.2
* **Nwpie.Foundation.*.SDK (1.9.1)**
  * fix: Use `AccountProfileBase` as default cache type for profile
  * fix: Cache.GetAsync<T> T should be solid class type instead of interface
  * pkge: Upgrade `Microsoft` package 3.1.6 -> 3.1.7
  * pkge: Upgrade `ServiceStack` package 5.9.0 -> 5.9.2

### 2020-07-29
* **Nwpie.Foundation.DataAccess.Database (1.8.0)**
  * fix: FlexibleCommandBuilder.CombinParameters() Regex.Replace issue
  * feat: Add Cache for logging ITokenDataModel in ServiceNodeUtils class
  * feat: Add ICommandBuilder EnableDynamicSection(string sectionName)
  * BREAKING CHANGE: ICommandExecutor SetParameterValue(string paramName, bool enable);

### 2020-07-27
* **Nwpie.Foundation.Http.Common (1.7.6)**
  * feat: Add Nwpie.Foundation.Http.Common
  * BREAKING CHANGE: Remove HttpRequest dependency
* **Nwpie.Foundation.Abstractions (1.4.4)**
  * pkge: Remove AspNetCore packages
  * BREAKING CHANGE: Remove HttpRequest dependency
* **Nwpie.Foundation.Common (1.7.6)**
  * pkge: Remove AspNetCore packages dependency
  * BREAKING CHANGE: Remove HttpRequest dependency
* **Nwpie.Foundation.Auth.Contract (1.4.4)**
  * pkge: Remove AspNetCore packages dependency

### 2020-07-20
* **Nwpie.Foundation.Auth.SDK (1.7.5)**
  * feat: TokenProfileMgr allow give specific sender apiName and apiKey
  * BREAKING CHANGE: Deny token which missing `Exp` value (Not allow null)

### 2020-07-16
* **Nwpie.Foundation.Abstractions (1.4.3)**
  * pkge: Upgrade `Microsoft` package 3.1.5 -> 3.1.6
* **Nwpie.Foundation.Common (1.7.4)**
  * pkge: Upgrade `Microsoft.Extensions.Caching.Memory` package 3.1.5 -> 3.1.6
* **Nwpie.Foundation.DataAccess.Database (1.7.4)**
  * pkge: Upgrade `MySql` package 8.0.20 -> 8.0.21
* **Nwpie.Foundation.Hosting.ServiceStack (1.7.4)**
  * pkge: Upgrade `Microsoft.Extensions.Logging.Console` package 3.1.5 -> 3.1.6

### 2020-07-02
* **Nwpie.Foundation.ServiceNode.ServiceStack (1.7.3)**
  * pkge: Upgrade `AutoMapper` to 10.0
  * fix: GetTokenDetail() for non-auth applications

### 2020-07-01
* **Nwpie.Foundation.Storage.S3 (1.7.2)**
  * test: Add S3 versioning UnitTest
  * feat: Add storage method for ListVersionsAsync() and CopyVersionToLatestAsync()

### 2020-06-30
* **Nwpie.Foundation.Auth.Contract (1.4.1)**
  * feat: Modify Contract for `IAccountProfile`, and `Versions` for Resource
* **Nwpie.Foundation.Commmon (1.7.1)**
  * feat: Add DictionaryExtension methods
* **Nwpie.Foundation.ServiceNode.ServiceStack (1.7.1)**
  * feat: Modify ExecuteAsync() for TokenFilterAsyncAttribute
  * chore: Log GetRequestRemoteIP() and GetRequestUserAgent() if exceptions

### 2020-06-15
* **Nwpie.Foundation.Abstractions (1.4.0)**
  * pkge: Upgrade `ServiceStack` and `Microsoft` packages
* **Nwpie.Foundation.Commmon (1.7.0)**
  * pkge: Upgrade `Microsoft` packages

### 2020-05-27
* **Nwpie.Foundation.Abstractions (1.3.1)**
  * chore: /DeleteMe, /CreateToken, etc accept `AccountId` parameter

* **Nwpie.Foundation.Abstractions (1.6.1)**
  * BREAKING CHANGE: Change env key `CARK_BASE_SERVICE_URL` to `SDK_CONFIG_SERVICE_URL`
  * BREAKING CHANGE: Change env key `CARK_BASE_SERVICE_URL`'s value from `https://api-dev.kevinw.net/foundation` to `https://api-dev.kevinw.net/foundation/configserver`

### 2020-05-26
* **Nwpie.Foundation.Common (1.6.0)**
  * feat: Integrates with the latest **Autofac** `5.2.0` version

### 2020-05-07
* **Nwpie.Foundation.Configuration.SDK (1.5.4)**
  * fix: DefaultRemoteConfigClient.GetLatest missing assigned arguments for `apiName` and `apiKey`

### 2020-04-29
* **Nwpie.Foundation.Common (1.5.3)**
  * fix: The way to check IsAsyncMethod
  * chore: Log ApiServiceEntry request and response without `await`

### 2020-04-26
* **Nwpie.Foundation.Hosting.ServiceStack.Extensions (1.5.2)**
  * feat: Add RegisterAsImplementedInterfaces() has same behavior from Autofac.ContainerBuilder.builder.RegisterAssemblyTypes<T>

### 2020-04-21
* **Nwpie.Foundation.Auth.SDK (1.5.1)**
  * fix: IsClientSourceChanged() should ignore checking if current IP is empty

### 2020-04-13
* **Nwpie.Foundation.Auth.SDK (1.5.0)**
  * feat: GetTokenDetail(), GetAccountId(), GetProfile() must give AuthExactFlagEnum flags

### 2020-02-11
* **Nwpie.Foundation.Common (1.3.5.9)**
  * chore: Remove Microsoft.AspNetCore.Http package
  * chore: Remove System.IdentityModel.Tokens.Jwt package
* **Nwpie.Foundation.S3Proxy.Lambda.Service (1.1.0)**
  * chore: ~/s3proxy/upload allow **POST** method

### 2020-01-22
* **Nwpie.Foundation.Common (1.3.5.8)**
  * feat: Add extension functions
    * ConfigurationExtension.ConfigServerRawValue()
    * ConfigServerRawValue.ConfigServerValue()
  * fix: Modify ApiUtility.HttpRequest
    * Set KeepAlive=false to prevent running out of sockets
* **Nwpie.Foundation.Configuration.SDK (1.3.5.8)**
  * feat: Load configs on location service
  * feat: Fail over to config server if got trouble communicating with location server
* **Nwpie.Foundation.DataAccess.Mapper (1.3.5.8)**
  * feat: Wrap AutoMapper
  * feat: Add mapper extension

### 2020-01-08
* **Nwpie.Foundation.Common (1.3.5.5)**
  * feat: Auto load .version file as default application version
* **Nwpie.xUnit**
  * chore: Modify RDS configs for all **production** databases

### 2019-12-31
* **Nwpie.Foundation.Common (1.3.5.4)**
  * BREAKING CHANGE: MessageQueue callback function signature changed:
    **From**: Action<string, string>
    **To**: Action<string, ICommandModel>
* **Nwpie.Foundation.Hosting.ServiceStack (1.3.5.4)**
  * Add RegisterEvent for IApplicationBuilder
* **Nwpie.xUnit**
  * chore: Modify production config for `foundation.aws.mysql.connectionstring.auth_db`
  * test: Add apikey for tlda.3dviewer.preprod and tlda.3dviewer.prod

### 2019-12-30
* **Nwpie.xUnit**
  * chore: Modify RDS configs for all dev databases
  * test: Support launchSettings.json in #DEBUG mode

### 2019-12-22
* **Nwpie.Foundation.Common (1.3.5.3)**
  * Add HttpUtils
* **Nwpie.Foundation.ServiceNode.HealthCheck (1.3.5.3)**
  * Const the route for healthcheck:
    [**Route**("/" + SNConst.HealthCheckController + "/HlckEchoRequest", "GET,POST")]
  * Add HealthCheckExtension.IsHealthCheckRequest(string rawUrl) to see if the is built-in healthchek url
* **Nwpie.Foundation.ServiceNode.ServiceStack (1.3.5.3)**
  * Added api.host and svc metrics to measurement service per request
