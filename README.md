# Nwpie.Foundation.NetCore.SDK

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-BSD%203--Clause-blue.svg)](LICENSE)
[![Build](https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK/actions/workflows/dotnet.yml/badge.svg)](https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK/actions/workflows/dotnet.yml)
[![GitHub stars](https://img.shields.io/github/stars/osisdie/Nwpie.Foundation.NetCore.SDK)](https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK/stargazers)
[![GitHub issues](https://img.shields.io/github/issues/osisdie/Nwpie.Foundation.NetCore.SDK)](https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK/issues)
[![Last commit](https://img.shields.io/github/last-commit/osisdie/Nwpie.Foundation.NetCore.SDK)](https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK/commits/main)

**Nwpie.Foundation.NetCore.SDK** is a comprehensive, modular SDK built for .NET Core (.NET 8) developers. It simplifies and enhances application development by providing an all-in-one solution with well-defined abstractions, robust utilities, and modularized components. This SDK is tailored for modern .NET Core projects and is fully supported with unit tests.

## Table of Contents
- [Features](#features)
- [Modules Overview](#modules-overview)
- [Quick Start](#quick-start)
- [Getting Started](#getting-started)
  - [Clone the Repository](#clone-the-repository)
  - [Explore Modules](#explore-modules)
  - [Run Unit Tests](#run-unit-tests)
- [Contributing](#contributing)
- [License](#license)

---

## Features
- **.NET Core (.NET 8) Compatibility**: Designed for modern .NET Core applications.
- **Modular SDK**: Use what you need with a modularized structure.
- **Comprehensive Tests**: Unit tests demonstrate usage and ensure reliability.
- **Extensive Utilities**: Includes tools for logging, configuration, data access, and more.

---

## Modules Overview
The SDK provides the following modules, located under the `src/` directory:

| Module Name         | Description                                                                 |
|---------------------|-----------------------------------------------------------------------------|
| **Abstractions**    | Contains interfaces and base classes for common functionality.              |
| **Auth**            | Handles authentication, authorization, and token management.                |
| **Caching**         | Provides caching utilities for in-memory and distributed caching.           |
| **Common**          | Includes general-purpose utilities and helper functions.                    |
| **Configuration**   | Simplifies application configuration management.                            |
| **DataAccess**      | Offers tools for database access and ORM integration.                       |
| **Extensions**      | Provides extension methods for common .NET types.                           |
| **Hosting**         | Supports application hosting and startup configurations.                    |
| **Http**            | Simplifies HTTP requests, API interactions, and REST client management.     |
| **Location**        | Provides service discovery environment mapping and hosting.                 |
| **Logging**         | Includes advanced logging utilities for structured logging.                 |
| **Measurement**     | Offers tools for performance monitoring and metrics collection.             |
| **MessageQueue**    | Supports message queuing and event-driven architectures.                    |
| **Notification**    | Handles email, SMS, and push notification services.                         |
| **ServiceNode**     | Provides utilities for service discovery and node management.               |
| **Storage**         | Simplifies file and blob storage operations.                                |

---

## Quick Start

### Caching

Register local or Redis caching in your DI container:

```csharp
// In-memory cache
services.AddLocalCache<ILocalCache>();
services.AddAsDefaultICache<ILocalCache>();

// Redis cache (requires Caching.Redis module)
services.AddRedisCache<IRedisCache>();
```

### Serialization

Register the default JSON serializer:

```csharp
services.AddDefaultSerializer<ISerializer>();
```

### Service Startup

A typical service `Startup.cs` wires up serialization, caching, and module-specific services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDefaultSerializer<ISerializer>();
    services.AddLocalCache<ILocalCache>();
    services.AddAsDefaultICache<ILocalCache>();

    // Add module-specific services
    services.AddConfigServer<IConfigServer>();
}
```

See the [samples/](samples/) directory for complete working examples (MiniSite, Serverless).

---

## Getting Started

### Clone the Repository
Clone the repository to explore the SDK:

```bash
git clone https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK.git
cd Nwpie.Foundation.NetCore.SDK
```

### Explore Modules
Modules are located in the `src/` directory. For example:

- `src/Abstractions`
- `src/Auth`
- ... and more.

Each module is self-contained and can be used independently or as part of the complete SDK.

### Run Unit Tests
Unit tests demonstrate how to use each module. Tests are located under `test/UnitTest`.

To run the tests:

1. Navigate to the root directory.
2. Use the following command:

```bash
dotnet test
```

This will execute all unit tests and verify the SDK's functionality.

---

## Contributing
We welcome contributions from the open-source community! See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

---

## License
This project is licensed under the BSD-3-Clause License. See the [LICENSE](LICENSE) file for details.

---

## Support
If you encounter any issues or have suggestions, feel free to open an [issue](https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK/issues) on GitHub.
