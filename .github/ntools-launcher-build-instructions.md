---
name: ntools-launcher-build-instructions
applyTo: "**"
description: "Build and test instructions for ntools-launcher project. Use when: working with the ntools-launcher project - always route build commands to 'nb build' and test commands to 'nb test'."
---

# ntools-launcher Build & Test Instructions

## Build Command

When the user asks to build, compile, or check the project:

1. **Always use**: `nb build` from the `c:\source\ntools-launcher` directory
2. **Never use**: `dotnet build` directly
3. The `nb build` command uses the custom Nbuild build system defined in `nbuild.targets`

Example: If user says "build the project", run:
```bash
cd c:\source\ntools-launcher
nb build
```

## Test Command

When the user asks to run tests, execute tests, or verify functionality:

1. **Always use**: `nb test` from the `c:\source\ntools-launcher` directory
2. **Never use**: `dotnet test` directly
3. The `nb test` command uses the custom Nbuild test system with code coverage collection
4. Test results include:
   - Pass/fail count
   - Code coverage metrics
   - Test execution times

Example: If user says "run tests", run:
```bash
cd c:\source\ntools-launcher
nb test
```

## Project Structure

- **Source Code**: `launcher/`, `LauncherTests/`
- **Build System**: `nbuild.targets`, `Nbuild.bat`
- **Configuration**: `ntools-launcher.sln`
- **Models**: `launcher/YamlLauncher/Models/`
- **Implementation**: `launcher/YamlLauncher/`

## Test Files Location

- **Unit Tests**: `LauncherTests/YamlLauncher/`
- **All tests must follow**: `*Tests.cs` naming convention
- **Test Framework**: MSTest (.NET 10.0)

## Default Behavior

- When building: Always check compilation first
- When testing: Always run full test suite via `nb test`
- Code coverage target: 90%+ for implementation files
