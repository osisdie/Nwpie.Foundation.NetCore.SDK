# Nwpie.Foundation.NetCore.SDK

[![.NET](https://img.shields.io/badge/.NET-8%20%7C%2010%20%7C%2011--preview-blue.svg)](https://dotnet.microsoft.com/download/dotnet)
[![NuGet](https://img.shields.io/nuget/v/Nwpie.Foundation.Abstractions.svg)](https://www.nuget.org/packages?q=Nwpie.Foundation)
[![netstandard2.1](https://img.shields.io/badge/netstandard-2.1-blue.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/actions/workflows/dotnet.yml/badge.svg)](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/actions/workflows/dotnet.yml)
[![Modules](https://img.shields.io/badge/modules-16-green.svg)](#modules-overview)
[![GitHub stars](https://img.shields.io/github/stars/osisdie/dotnet-nwpie-foundation-sdk)](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/stargazers)
[![GitHub issues](https://img.shields.io/github/issues/osisdie/dotnet-nwpie-foundation-sdk)](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/issues)
[![Last commit](https://img.shields.io/github/last-commit/osisdie/dotnet-nwpie-foundation-sdk)](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/commits/main)

> **English** | [繁體中文](README.zh-TW.md)

**Nwpie.Foundation.NetCore.SDK** is a comprehensive, modular SDK built for .NET developers. It simplifies and enhances application development by providing an all-in-one solution with well-defined abstractions, robust utilities, and modularized components. Core libraries target `netstandard2.1` for broad compatibility, while endpoint and test projects multi-target `net8.0` and `net10.0`. Fully supported with unit tests.

## Table of Contents
- [Features](#features)
- [Installation](#installation)
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
- **Multi-target .NET 8, .NET 10 & .NET 11 (preview)**: Endpoint and test projects target `net8.0` and `net10.0`. CI also validates against .NET 11 preview. Core libraries use `netstandard2.1` for broad compatibility.
- **Modular SDK**: Use what you need with a modularized structure.
- **NuGet Ready**: Install individual modules via NuGet for easy integration.
- **Comprehensive Tests**: Unit tests demonstrate usage and ensure reliability.
- **Extensive Utilities**: Includes tools for logging, configuration, data access, and more.

---

## Installation

Install individual modules via NuGet:

```bash
dotnet add package Nwpie.Foundation.Abstractions
dotnet add package Nwpie.Foundation.Common
dotnet add package Nwpie.Foundation.Caching.Common
dotnet add package Nwpie.Foundation.Caching.Redis
dotnet add package Nwpie.Foundation.Auth.SDK
dotnet add package Nwpie.Foundation.Http.Common
```

Or search for all available packages:

```bash
dotnet nuget search Nwpie.Foundation
```

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
git clone https://github.com/osisdie/dotnet-nwpie-foundation-sdk.git
cd dotnet-nwpie-foundation-sdk
```

### Explore Modules
Modules are located in the `src/` directory. For example:

- `src/Abstractions`
- `src/Auth`
- ... and more.

Each module is self-contained and can be used independently or as part of the complete SDK.

### Run Unit Tests
Unit tests demonstrate how to use each module. Tests are located under `tests/UnitTest`.

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
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Support
If you encounter any issues or have suggestions, feel free to open an [issue](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/issues) on GitHub.
