# NuGet Publishing Guide

## Prerequisites

- .NET 8 SDK (or later)
- A [NuGet.org](https://www.nuget.org/) account
- A NuGet API key with **Push** scope for `Nwpie.Foundation.*`

## Automated Publishing (GitHub Actions)

Publishing is automated via `.github/workflows/nuget-publish.yml`.

### Setup

1. Create an API key at https://www.nuget.org/account/apikeys
   - **Key Name**: e.g. `dotnet-nwpie-foundation-sdk`
   - **Glob Pattern**: `Nwpie.Foundation.*`
   - **Scopes**: Push
2. Add the key as a **Repository secret** in GitHub:
   - Go to **Settings > Secrets and variables > Actions**
   - Name: `NUGET_API_KEY`
   - Value: your API key

### Trigger

The workflow runs automatically when:
- A **GitHub Release** is published
- Manually via **workflow_dispatch** in the Actions tab

### What it does

1. Restores and builds the solution
2. Runs `dotnet pack` — only library projects are packed (`IsPackable=false` excludes endpoints, tests, samples)
3. Pushes all `.nupkg` files to NuGet.org with `--skip-duplicate`

## Manual Publishing

```bash
# Build
dotnet build --configuration Release

# Pack (outputs to ./nupkgs)
dotnet pack --configuration Release --no-build --output ./nupkgs

# Push all packages
dotnet nuget push "./nupkgs/*.nupkg" \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

## Package List

All packages follow the naming convention `Nwpie.Foundation.<Module>`:

| Package | Description |
|---------|-------------|
| `Nwpie.Foundation.Abstractions` | Interfaces, models, enums |
| `Nwpie.Foundation.Common` | Utilities and helpers |
| `Nwpie.Foundation.Auth.Contract` | Auth API contracts |
| `Nwpie.Foundation.Auth.SDK` | Auth SDK implementation |
| `Nwpie.Foundation.Caching.Common` | Caching abstractions |
| `Nwpie.Foundation.Caching.LocalCache` | In-memory cache |
| `Nwpie.Foundation.Caching.Redis` | Redis cache |
| `Nwpie.Foundation.Configuration.SDK` | Configuration management |
| `Nwpie.Foundation.DataAccess.Database` | Database access |
| `Nwpie.Foundation.DataAccess.Mapper` | Object mapping |
| `Nwpie.Foundation.Extensions.xUnit` | xUnit extensions |
| `Nwpie.Foundation.Hosting.ServiceStack` | ServiceStack hosting |
| `Nwpie.Foundation.Http.Common` | HTTP client utilities |
| `Nwpie.Foundation.Location.Contract` | Location contracts |
| `Nwpie.Foundation.Location.Core` | Location core logic |
| `Nwpie.Foundation.Location.SDK` | Location SDK |
| `Nwpie.Foundation.Logging.ElasticSearch` | ElasticSearch logging |
| `Nwpie.Foundation.Logging.Log4net` | Log4net adapter |
| `Nwpie.Foundation.Logging.NLog` | NLog adapter |
| `Nwpie.Foundation.Measurement.CloudWatch` | CloudWatch metrics |
| `Nwpie.Foundation.Measurement.Common` | Measurement abstractions |
| `Nwpie.Foundation.Measurement.Core` | Measurement core |
| `Nwpie.Foundation.Measurement.SDK` | Measurement SDK |
| `Nwpie.Foundation.MessageQueue.Common` | MQ abstractions |
| `Nwpie.Foundation.MessageQueue.Factory` | MQ factory |
| `Nwpie.Foundation.MessageQueue.RabbitMQ` | RabbitMQ adapter |
| `Nwpie.Foundation.MessageQueue.SNS` | AWS SNS adapter |
| `Nwpie.Foundation.MessageQueue.SQS` | AWS SQS adapter |
| `Nwpie.Foundation.Notification.Common` | Notification abstractions |
| `Nwpie.Foundation.Notification.Contract` | Notification contracts |
| `Nwpie.Foundation.Notification.SDK` | Notification SDK |
| `Nwpie.Foundation.Notification.Sendgrid` | Sendgrid adapter |
| `Nwpie.Foundation.Notification.Smtp` | SMTP adapter |
| `Nwpie.Foundation.ServiceNode.HealthCheck` | Health check |
| `Nwpie.Foundation.ServiceNode.SDK` | ServiceNode SDK |
| `Nwpie.Foundation.ServiceNode.ServiceStack` | ServiceStack integration |
| `Nwpie.Foundation.ServiceNode.ServiceStack.DepdencyInjection` | ServiceStack DI |
| `Nwpie.Foundation.Storage.Common` | Storage abstractions |
| `Nwpie.Foundation.Storage.S3` | AWS S3 adapter |

## Version Management

Version is centralized in `Directory.Build.props`:

```xml
<VersionPrefix>1.0.0</VersionPrefix>
```

To release a new version, update `VersionPrefix` and create a new GitHub Release.
