# Test-Framework Integration: YAML Schema Extension

**Purpose:** Extend YAML launcher schema to support test-framework's output capture, assertions, and result passing while maintaining 100% backward compatibility.

**Key Principle:** All new features are **optional**. Simple execution mode (current YAML) works exactly as before. Test validation mode (new) is opt-in via assertions and extractions.

---

## Backward Compatibility Guarantee

### Current Simple YAML (Continues to Work)

```yaml
# This will always work - no changes needed
version: "1.0"

task:
  path: "sdo.exe"
  arguments: "install --name MyApp"
  timeout: 120000

execution:
  verbose: true
```

### Extended YAML (New Features)

```yaml
# New optional features don't break anything
version: "1.0"

task:
  path: "sdo.exe"
  arguments: "install --name MyApp"
  timeout: 120000
  
  # NEW: Optional assertion validation
  expectedReturnCode: 0
  assertions:
    - type: output_contains
      value: "Successfully installed"
  
  # NEW: Optional variable extraction
  extractVariables:
    - name: "app_version"
      pattern: "Version: (\\d+\\.\\d+\\.\\d+)"
      groupIndex: 1

execution:
  verbose: true
```

**Result:** Both YAML configurations are valid. Old code using simple mode unaffected. New code can use assertions/extractions when needed.

---

## Design Philosophy

### Level 1: Simple Execution (Current - No Changes)
```yaml
task:
  path: "app.exe"
  arguments: "--flag value"
```
✅ Just runs the executable, captures output, returns result.

### Level 2: Return Code Validation (New - Optional)
```yaml
task:
  path: "app.exe"
  arguments: "--flag value"
  expectedReturnCode: 0  # NEW
```
✅ Runs executable, validates return code matches expectation, fails if mismatch.

### Level 3: Output Assertions (New - Optional)
```yaml
task:
  path: "app.exe"
  arguments: "--flag value"
  expectedReturnCode: 0
  assertions:              # NEW
    - type: output_contains
      value: "Success"
```
✅ Runs executable, validates return code, validates output contains string.

### Level 4: Variable Extraction (New - Optional)
```yaml
tasks:
  - name: "step1"
    path: "get-version.exe"
    expectedReturnCode: 0
    extractVariables:      # NEW
      - name: "version"
        pattern: "Version: (\\d+\\.\\d+\\.\\d+)"
        groupIndex: 1
  
  - name: "step2"
    path: "install.exe"
    arguments: "--version $(version)"  # Uses variable from step1
    dependencies: ["step1"]
    expectedReturnCode: 0
```
✅ Runs step1, extracts variable from output, uses variable in step2 arguments.

---

## Extended StepConfig Schema

### New Optional Properties for Test Validation

```yaml
task:
  # ... existing properties (path, arguments, timeout, etc.) ...
  
  # NEW: Return code validation
  expectedReturnCode: 0                    # Optional: validate exit code
  
  # NEW: Output assertions
  assertions:                              # Optional: validate output
    - type: "output_contains"
      value: "string to find"
      caseInsensitive: false
      description: "Check for success message"
    
    - type: "output_not_contains"
      value: "error"
      description: "Ensure no errors"
    
    - type: "output_matches"
      pattern: "^Success.*$"
      description: "Output matches regex"
    
    - type: "output_equals"
      value: "exact output"
      caseInsensitive: false
      description: "Exact output match"
    
    - type: "custom_json"
      jsonPath: "$.status"
      expectedValue: "success"
      description: "JSON path validation"
  
  # NEW: Variable extraction from output
  extractVariables:                        # Optional: extract values
    - name: "branch_name"
      pattern: "Branch: (\\S+)"            # Regex with capture group
      groupIndex: 1                        # Which capture group (1-based)
      caseInsensitive: false
      description: "Extract branch name from output"
    
    - name: "app_version"
      pattern: "Version:\\s+(\\d+\\.\\d+\\.\\d+)"
      groupIndex: 1
      description: "Extract semantic version"
```

### New StepConfig Properties Table

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `expectedReturnCode` | int | No | - | Expected process exit code (0 by default); if mismatch, task fails |
| `assertions` | Assertion[] | No | [] | List of output assertions to validate |
| `extractVariables` | VariableExtraction[] | No | [] | List of regex patterns to extract variables from output |

### Assertion Object

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `type` | string | Yes | - | Assertion type: "output_contains", "output_not_contains", "output_matches", "output_equals", "exit_code", "file_exists", "custom_json" |
| `value` | string | No | - | For "output_contains", "output_not_contains", "output_equals", "exit_code" types |
| `pattern` | string | No | - | For "output_matches" type (regex pattern) |
| `jsonPath` | string | No | - | For "custom_json" type (JPath expression) |
| `expectedValue` | string\|int\|bool | No | - | For "custom_json" type (value to match) |
| `caseInsensitive` | bool | No | true | For string comparisons |
| `description` | string | No | "" | Human-readable assertion description |

### VariableExtraction Object

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `name` | string | Yes | - | Variable name (used in substitution as `$(name)`) |
| `pattern` | string | Yes | - | Regex pattern with capture group |
| `groupIndex` | int | No | 1 | Capture group number (1-based indexing) |
| `caseInsensitive` | bool | No | false | Case-insensitive regex matching |
| `description` | string | No | "" | Human-readable description |

---

## Real-World Examples

### Example 1: Simple App Verification (Level 2)

```yaml
version: "1.0"
description: "Verify app installed successfully"

task:
  path: "C:\\Program Files\\MyApp\\app.exe"
  arguments: "--version"
  expectedReturnCode: 0

execution:
  verbose: true
```

**Behavior:**
- Runs app with `--version` flag
- Checks that exit code is exactly 0
- Fails if exit code is not 0
- Captures and returns output

---

### Example 2: Output Validation (Level 3)

```yaml
version: "1.0"
description: "Verify installation output"

task:
  path: "installer.exe"
  arguments: "--install --verbose"
  expectedReturnCode: 0
  assertions:
    - type: "output_contains"
      value: "Installation complete"
      description: "Verify completion message"
    
    - type: "output_not_contains"
      value: "error"
      caseInsensitive: true
      description: "Ensure no error messages"

execution:
  verbose: true
```

**Behavior:**
- Runs installer
- Checks exit code is 0
- Validates output contains "Installation complete"
- Validates output does NOT contain "error" (case-insensitive)
- Fails if any assertion fails with helpful error message

---

### Example 3: Variable Extraction & Passing (Level 4)

```yaml
version: "1.0"
description: "Multi-step deployment with variable flow"

tasks:
  - name: "get-latest-version"
    path: "gh.exe"
    arguments: "release list --repo mycompany/myapp --json --limit 1"
    expectedReturnCode: 0
    extractVariables:
      - name: "version"
        pattern: '"tag_name":\\s*"v([\\d.]+)"'
        groupIndex: 1
        description: "Extract version number from JSON output"

  - name: "download-release"
    path: "downloader.exe"
    arguments: "--version $(version) --output release.zip"
    dependencies: ["get-latest-version"]
    expectedReturnCode: 0
    assertions:
      - type: "file_exists"
        value: "release.zip"
        description: "Verify download completed"

  - name: "verify-signature"
    path: "verifier.exe"
    arguments: "--file release.zip"
    dependencies: ["download-release"]
    expectedReturnCode: 0
    assertions:
      - type: "output_contains"
        value: "Signature valid"
        description: "Verify digital signature"

  - name: "extract-and-install"
    path: "powershell.exe"
    arguments: "-Command Expand-Archive -Path release.zip -DestinationPath app"
    dependencies: ["verify-signature"]
    expectedReturnCode: 0

execution:
  mode: "sequential"
  stopOnFirstError: true
  verbose: true
```

**Execution Flow:**
1. `get-latest-version`: Runs GitHub CLI, extracts version `1.2.3` from JSON
2. `download-release`: Uses `$(version)` to download `v1.2.3`, verifies file exists
3. `verify-signature`: Validates the downloaded file signature
4. `extract-and-install`: Extracts and installs application

**Result Passing:** Version from step 1 automatically available in step 2-4 via `$(version)`.

---

### Example 4: Complex Test Matrix (Level 4 + Parallel)

```yaml
version: "1.0"
description: "Run tests on multiple frameworks"

variables:
  TestProject: "C:\\src\\MyApp.Tests\\MyApp.Tests.csproj"
  ProjectPath: "C:\\src"

tasks:
  - name: "restore"
    path: "dotnet.exe"
    arguments: "restore"
    workingDirectory: "$(ProjectPath)"
    expectedReturnCode: 0

  - name: "build"
    path: "dotnet.exe"
    arguments: "build --configuration Release --no-restore"
    workingDirectory: "$(ProjectPath)"
    dependencies: ["restore"]
    expectedReturnCode: 0

  - name: "test_net6"
    path: "dotnet.exe"
    arguments: "test $(TestProject) --framework net6.0 --no-build --logger trx"
    workingDirectory: "$(ProjectPath)"
    dependencies: ["build"]
    expectedReturnCode: 0
    assertions:
      - type: "output_contains"
        value: "passed"
        caseInsensitive: true
      - type: "output_not_contains"
        value: "failed"
        caseInsensitive: true

  - name: "test_net8"
    path: "dotnet.exe"
    arguments: "test $(TestProject) --framework net8.0 --no-build --logger trx"
    workingDirectory: "$(ProjectPath)"
    dependencies: ["build"]
    expectedReturnCode: 0
    assertions:
      - type: "output_contains"
        value: "passed"
        caseInsensitive: true
      - type: "output_not_contains"
        value: "failed"
        caseInsensitive: true

execution:
  mode: "sequential"
  stopOnFirstError: false  # Run all tests even if one fails
  verbose: true
```

---

### Example 5: test-framework Integration Pattern

```yaml
version: "1.0"
description: "SDO test scenario - Multi-step with assertions"

tasks:
  - name: "get-work-item"
    path: "sdo.exe"
    arguments: "wi list --top 1 --json"
    expectedReturnCode: 0
    extractVariables:
      - name: "work_item_id"
        pattern: '"id":\\s*(\d+)'
        groupIndex: 1

  - name: "start-work"
    path: "sdo.exe"
    arguments: "wi start $(work_item_id) --verbose"
    dependencies: ["get-work-item"]
    expectedReturnCode: 0
    assertions:
      - type: "output_contains"
        value: "Branch:"
        description: "Verify branch creation"
    extractVariables:
      - name: "branch_name"
        pattern: "Branch: (\\S+)"
        groupIndex: 1

  - name: "verify-branch"
    path: "git.exe"
    arguments: "branch -a"
    expectedReturnCode: 0
    assertions:
      - type: "output_contains"
        value: "$(branch_name)"
        description: "Verify branch exists in git"

  - name: "cleanup"
    path: "git.exe"
    arguments: "branch -D $(branch_name)"
    dependencies: ["verify-branch"]
    expectedReturnCode: 0
    continueOnError: true  # Always cleanup, even if tests fail

execution:
  mode: "sequential"
  stopOnFirstError: false
  verbose: true
```

---

## Migration Path: test-framework → YAML Launcher

### Current test-framework YAML

```yaml
# tests/sdo-scenario.yaml (test-framework format)
commands:
  - step: 1
    name: "Get Work Item"
    command: "sdo.exe"
    args: ["wi", "list", "--top", "1", "--json"]
    expected_exit_code: 0
    variable_extractions:
      - name: "work_item_id"
        pattern: '"id":\s*(\d+)'
        group_index: 1
```

### Equivalent YAML Launcher Config

```yaml
# deploy/sdo-scenario.yaml (YAML launcher format)
version: "1.0"

tasks:
  - name: "Get Work Item"
    path: "sdo.exe"
    arguments: "wi list --top 1 --json"
    expectedReturnCode: 0
    extractVariables:
      - name: "work_item_id"
        pattern: '"id":\s*(\d+)'
        groupIndex: 1
```

**Migration Cost:** Minimal - just rename properties (snake_case → camelCase) and add version/executables wrapper.

---

## Implementation Strategy

### Phase 1A: Parsing (Extends Phase 1)
- Add optional properties to YAML parser (expectedReturnCode, assertions, extractVariables)
- All new properties are optional - parsing works for both old and new YAML
- No breaking changes to existing code

### Phase 1B: Validation (Extends Phase 1)
- Add StepConfig validation for assertions
- Ensure extractVariables patterns are valid regex
- Report helpful error messages

### Phase 2: Assertion Engine (New)
- Create AssertionValidator class
- Implement each assertion type (output_contains, output_matches, etc.)
- Return detailed validation results

### Phase 3: Variable Extraction (New)
- Create VariableExtractor class
- Regex-based pattern matching
- Supports numbered capture groups
- Stores extracted variables in execution context

### Phase 4: Result Passing (Extends Phase 2-3)
- Pass extracted variables to next step's execution context
- Support $(variable_name) substitution in arguments, workingDirectory, environment vars
- Works for both sequential and parallel pipelines

---

## Detailed Assertion Types

### Type: `output_contains`
```yaml
assertions:
  - type: "output_contains"
    value: "Success message"
    caseInsensitive: false
    description: "Check for success"
```
- Checks if stdout contains exact string
- Optional: case-insensitive matching

### Type: `output_not_contains`
```yaml
assertions:
  - type: "output_not_contains"
    value: "Error:"
    caseInsensitive: true
    description: "No error messages"
```
- Checks that stdout does NOT contain string
- Useful for negative testing

### Type: `output_matches`
```yaml
assertions:
  - type: "output_matches"
    pattern: "^Completed in \\d+ ms$"
    caseInsensitive: false
    description: "Verify completion format"
```
- Matches stdout against regex pattern
- Full regex support

### Type: `output_equals`
```yaml
assertions:
  - type: "output_equals"
    value: "OK"
    caseInsensitive: false
    description: "Exact output match"
```
- Entire stdout must exactly match value
- No regex, plain string comparison

### Type: `exit_code`
```yaml
assertions:
  - type: "exit_code"
    value: 0
    description: "Process succeeded"
```
- Validates process exit code
- Alternative to expectedReturnCode (more explicit)

### Type: `file_exists`
```yaml
assertions:
  - type: "file_exists"
    value: "$(InstallPath)\\app.exe"
    description: "Installation successful"
```
- Checks file exists on disk
- Supports variable substitution

### Type: `custom_json`
```yaml
assertions:
  - type: "custom_json"
    jsonPath: "$.result.status"
    expectedValue: "success"
    description: "API response successful"
```
- Parses JSON output and validates with JSONPath
- Flexible value matching

---

## Error Handling & Messaging

### Assertion Failure Example

**YAML:**
```yaml
tasks:
  path: "installer.exe"
  arguments: "--install"
  expectedReturnCode: 0
  assertions:
    - type: "output_contains"
      value: "Installation complete"
```

**Error Output (When Assertion Fails):**
```
❌ Task 'installer' failed
   Expected return code: 0
   Actual return code: 0 ✓
   
   Assertion 1 FAILED: output_contains
   Expected substring: "Installation complete"
   Actual output:
   ─────────────────────────────────────────
   Preparing installation...
   Downloading files...
   Setup failed: Insufficient permissions
   ─────────────────────────────────────────
   
   Suggestion: Run with elevated privileges
```

### Variable Extraction Failure

**YAML:**
```yaml
extractVariables:
  - name: "app_version"
    pattern: "Version: (\\d+\\.\\d+\\.\\d+)"
    groupIndex: 1
```

**Error Output (When Pattern Doesn't Match):**
```
⚠️  Warning: Variable extraction failed
   Variable name: app_version
   Pattern: Version: (\d+\.\d+\.\d+)
   
   Pattern not found in output
   Output searched (first 500 chars):
   ─────────────────────────────────────────
   Installation complete
   Please restart your computer
   ─────────────────────────────────────────
   
   Suggestion: Verify output format matches pattern
```

---

## API Usage Examples

### C# Usage (Simple Execution - Unchanged)

```csharp
// This continues to work - no changes
var executor = new StepExecutor();
var result = await executor.LaunchAsync("simple-app.yaml");
```

### C# Usage (With Assertions)

```csharp
// New capability - opt-in
var executor = new StepExecutor();
var result = await executor.LaunchAsync("test-app.yaml");

if (!result.Success)
{
    foreach (var failure in result.AssertionFailures)
    {
        Console.WriteLine($"❌ {failure.AssertionType} failed: {failure.Message}");
    }
}
```

### C# Usage (Multi-Step with Variables)

```csharp
var executor = new StepExecutor();
var result = await executor.LaunchAsync(\"pipeline.yaml\");

// Access extracted variables
var variables = result.ExtractedVariables;
var appVersion = variables["app_version"];
Console.WriteLine($"Deployed version: {appVersion}");
```

---

## Success Criteria

- ✅ All current simple YAML configs work unchanged
- ✅ New assertions/extractions are optional
- ✅ Zero breaking changes to existing API
- ✅ test-framework scenario can be described in YAML
- ✅ Variables extracted from one step available in next step
- ✅ Helpful error messages for assertion failures
- ✅ Compatible with both sequential and parallel modes
- ✅ Consistent with test-framework naming/behavior patterns

---

## Next Steps

1. **Update YAML Schema Documentation** - Add new properties to yaml-schema-reference.md
2. **Extend Model Classes** (Phase 1B) - Add StepConfig.expectedReturnCode, assertions[], extractVariables[]
3. **Implement Assertion Engine** (Phase 2) - AssertionValidator class with all types
4. **Implement Variable Extraction** (Phase 3) - VariableExtractor with regex support
5. **Update Result Objects** - Add AssertionResults, ExtractedVariables to LaunchResult
6. **Add Tests** - Unit tests for each assertion type and extraction pattern
7. **Update Documentation** - Add real-world examples and migration guide

