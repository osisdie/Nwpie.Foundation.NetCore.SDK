# Security Policy

## Supported Versions

| Version | .NET Target      | Supported          |
|---------|------------------|--------------------|
| 1.0.x   | netstandard2.1   | Yes                |
| 1.0.x   | net8.0           | Yes                |
| 1.0.x   | net10.0          | Yes                |
| -       | net11.0 (preview)| No (preview only)  |

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly:

1. **Do NOT open a public GitHub issue** for security vulnerabilities
2. Use [GitHub Security Advisories](https://github.com/osisdie/dotnet-nwpie-foundation-sdk/security/advisories/new) to report privately
3. Alternatively, contact the maintainer directly

### What to expect

- Acknowledgment within 48 hours
- Status update within 7 days
- We will work with you to understand and address the issue

## Security Best Practices

When using this SDK:

- Keep dependencies up to date
- Never commit secrets, API keys, or credentials to source control
- Use the `.env.example` file as a template; store actual secrets in environment variables or a secrets manager
- Review the [CONTRIBUTING.md](CONTRIBUTING.md) for secure development guidelines
