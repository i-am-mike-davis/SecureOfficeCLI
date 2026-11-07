# OfficeCLI Test Suite

xUnit test project for OfficeCLI.

## Quick Start

```bash
# Run all tests
dotnet test

# Run tests matching a trait
dotnet test --filter "Category=Security"
dotnet test --filter "Component=UpdateChecker"
```

## Namespace Structure

Tests are organized by concern at the namespace level:

| Namespace | Purpose |
|---|---|
| `OfficeCli.Tests` | Umbrella — no tests live here directly |
| `OfficeCli.Tests.Security` | Security-related tests |

Each namespace maps to a physical subdirectory under this project root.

## xUnit Traits

Tests use xUnit traits to enable filtered execution:

| Trait | Key | Values |
|---|---|---|
| `Category` | `Security` | Security-related tests |
| `Component` | `UpdateChecker` | Tests for a specific component |

## Project Structure

```
officecli.Tests/
├── officecli.Tests.csproj
├── README.md                          ← you are here
└── Security/
    └── UpdateCheckerTests.cs          ← security patch tests
```
