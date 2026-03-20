# Contributing to Nwpie.Foundation.NetCore.SDK

Thank you for your interest in contributing! Here's how to get started.

## Development Setup

```bash
git clone https://github.com/osisdie/Nwpie.Foundation.NetCore.SDK.git
cd Nwpie.Foundation.NetCore.SDK
dotnet restore
dotnet build
dotnet test
```

**Prerequisites**: .NET 8 SDK

## How to Contribute

1. **Fork** the repository
2. **Create a branch**: `git checkout -b feature/my-feature`
3. **Make your changes** and add tests where applicable
4. **Run tests**: `dotnet test`
5. **Submit a pull request** with a clear description of your changes

## Project Structure

```
src/            # SDK modules (each module is self-contained)
tests/          # Unit and integration tests
samples/        # Sample applications (MiniSite, Serverless)
docker/         # Docker configurations
build/          # Build scripts and configurations
```

## Coding Guidelines

- **Library projects** target `netstandard2.1` for broad compatibility
- **Application projects** (endpoints, lambdas) target `net8.0`
- Follow existing naming conventions and code style
- Add unit tests for new functionality
- Keep modules self-contained and loosely coupled

## Reporting Issues

- Use the [Bug Report](.github/ISSUE_TEMPLATE/bug_report.md) template for bugs
- Use the [Feature Request](.github/ISSUE_TEMPLATE/feature_request.md) template for new ideas

## License

By contributing, you agree that your contributions will be licensed under the [BSD-3-Clause License](LICENSE).
