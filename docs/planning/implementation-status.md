# YAML Launcher Implementation Status

**Last reviewed:** September 18, 2026  
**Source of truth:** `launcher/YamlLauncher/` and `LauncherTests/YamlLauncher/`

This document records what is implemented in the repository today. Design proposals and future work are archived under `docs/planning/archive/` and must not be treated as supported behavior.

## Implemented

### Configuration loading and validation

- YAML parsing through YamlDotNet.
- Loading from files, strings, and readable streams.
- `steps:`, `tasks:`, and `apps:` aliases map to the same `LauncherConfig.Steps` collection.
- Required version, step name, and executable path validation.
- Validation of expected return codes, dependencies, assertion patterns, variables, and variable extraction patterns.
- Actionable `LauncherConfigException` errors, including YAML line and column information where available.

### Single-step execution

- `StepExecutor.LaunchAsync(LauncherConfig, int)` executes one selected step.
- Standard output and standard error capture.
- Working-directory validation.
- Global environment variables merged with step-specific environment variables.
- Optional administrator-elevation enforcement through `StepConfig.RequireElevation`.
- Expected exit-code handling.
- Per-execution timeout handling through `ExecutionSettings.Timeout`.
- Actual Windows Authenticode verification through `StepConfig.RequireSignature` and `SignatureVerifier`.
- Structured `LaunchResult` and `ExecutionResult` results with timing and output data.

### YAML test runner

`NtoolsLauncherTestRunner` provides additional orchestration for YAML test metadata:

- Discovers and runs named YAML test files.
- Executes steps in YAML order.
- Supports `stopOnFirstError`.
- Extracts variables from step output using regular expressions.
- Substitutes extracted variables in later arguments using `{variableName}` syntax.
- Evaluates these assertion types: `exit_code`, `output_contains`, `output_not_contains`, and `output_matches`.
- Runs all discovered tests in deterministic name order.

### Existing launcher behavior

- The existing process-based launcher APIs remain available.
- File download, locking, signature, and logging behavior remain separate from the YAML step executor.
- The deprecated `ServicePointManager` certificate probe has been removed in favor of the existing `HttpClientHandler` callback.

## Partial or limited support

These features exist in models or validation, but are not implemented as general-purpose orchestration by `StepExecutor`:

- `StepConfig.Dependencies` is validated, but no dependency graph is resolved and dependencies do not reorder execution.
- `StepConfig.ContinueOnError` is modeled, but the core executor does not run a pipeline that applies it.
- `ExecutionSettings.Mode` and `MaxConcurrency` are modeled, but there is no parallel execution engine.
- `LauncherConfig.Variables` and step environment values are passed to processes; general `$(variable)` substitution is not provided by `StepExecutor`.
- Assertions and variable extraction are evaluated by `NtoolsLauncherTestRunner`, not by `StepExecutor` itself.
- `LaunchResult.ExtractedVariables` is modeled, while extraction is currently collected per step by the test runner.

## Not implemented

The following remain future work:

- General multi-step orchestration API.
- Dependency-aware sequential orchestration.
- Parallel execution and concurrency throttling.
- General variable substitution engine, including recursive substitution and predefined variables.
- Retry policies and exponential backoff.
- Pre-execution and post-execution hooks.
- General output filtering and log parsing.
- A published YAML-specific NuGet package/API beyond the current launcher project.

## Verification

The implementation is covered by tests under `LauncherTests/YamlLauncher/`, including:

- Configuration model and alias behavior.
- YAML loading from strings, files, and streams.
- Validation failures and dependency validation.
- Single-step execution, output capture, timeouts, environment variables, working directories, exit codes, and signature behavior.
- Required-elevation enforcement and Windows Authenticode failure behavior.
- Test-runner variable extraction, substitution, assertions, and fail-fast behavior.

Run the repository build and test commands from the project documentation before changing the status in this file. Update this document whenever a feature moves between sections.

## Archived material

The former design, roadmap, terminology, architecture, and integration documents are retained in [archive](archive/README.md) for historical reference and future release planning. They describe proposals and examples, not the current implementation contract.
