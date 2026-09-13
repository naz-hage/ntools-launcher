---
name: sdo-build
description: "Build the ntools-launcher project using the custom Nbuild system. Use when: user says 'build project', 'build', 'compile', or needs to compile the C# code."
---

# sdo solution

Use `sdo solution` to compile the ntools-launcher project with the custom Nbuild build system.

## When to Use

- User says "build project" or "build the code"
- Code changes need to be compiled
- Need to verify compilation before running tests

## How to Run

Run the following command in the terminal:

```bash
sdo solution
```

This command:
- Compiles the C# project
- Runs the custom Nbuild system from the project directory
- Returns build status and any compilation errors

## Checking Build Results

After running `sdo solution`, check the results in `nbuild.log`:

```bash
cat nbuild.log
```

Look for:
- **Success**: "Build succeeded" or "BUILD_SUCCEEDED"
- **Failures**: Error messages starting with "error" or "ERROR"
- **Warnings**: Warning messages that need addressing
- **Build Summary**: Section showing overall build status

## Location

Run from: `c:\source\ntools-launcher`

Results logged to: `c:\source\ntools-launcher\nbuild.log`
