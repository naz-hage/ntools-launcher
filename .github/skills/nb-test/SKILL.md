---
name: nb-test
description: "Run tests for the ntools-launcher project using the custom Nbuild test system. Use when: user says 'run tests', 'test', 'execute tests', or needs to verify that code works correctly."
---

# NB Test

Use `nb test` to run all unit tests in the ntools-launcher project with the custom Nbuild test system.

## When to Use

- User says "run tests", "test the code", or "execute tests"
- Need to verify that recent code changes work correctly
- Want to check test coverage and results

## How to Run

Run the following command in the terminal:

```bash
nb test
```

This command:
- Executes all unit tests in the project
- Uses the custom Nbuild test system
- Reports test results (passed/failed counts)
- Collects code coverage metrics

## Test Files

Tests are located in:
- `LauncherTests/` - All unit tests
- Test projects follow the naming convention: `*Tests.cs`

## Checking Test Results

After running `nb test`, check the results in `nbuild.log`:

```bash
cat nbuild.log
```

Look for:
- **Test Summary**: "Test Run Passed" or "Test Run Failed"
- **Test Count**: "Total tests:", "Passed:", "Failed:", "Skipped:"
- **Pass Rate**: Total passed vs. failed count
- **Coverage Report**: Code coverage percentage and location
- **Failures**: Details of any failed tests with error messages
- **Duration**: Total test execution time

## Location

Run from: `c:\source\ntools-launcher`

Results logged to: `c:\source\ntools-launcher\nbuild.log`
