# Architectural Decisions & Tradeoffs

**Date:** May 25, 2026  
**Purpose:** Document key design choices and reasoning  
**Audience:** Architecture Review, Future Maintainers

---

## ADR-001: Optional Schema Properties for Backward Compatibility

### Decision
All test-framework features (assertions, variable extraction) are **optional** properties in ExecutableConfig. The YAML schema will support both minimal and enhanced configurations.

### Context
- Need to unify three codebases' capabilities
- Must maintain 100% backward compatibility
- Different use cases require different feature levels (simple execution vs. complex pipelines)

### Options Considered

**Option A: Single Required Schema** ❌
```yaml
# All properties required - breaks backward compatibility
task:
  name: "required"
  path: "required"
  expectedReturnCode: 0      # Required
  assertions: []              # Required
  extractVariables: []        # Required
```
**Pros:** Consistent, no hidden complexity
**Cons:** Breaks existing YAML configs, higher barrier to adoption

**Option B: Multiple Schema Versions** ❌
```yaml
schemaVersion: "1.0"    # For simple execution
schemaVersion: "2.0"    # For assertions/extraction
```
**Pros:** Clear separation of concerns
**Cons:** Complex parser, validation logic duplicated, confusing for users

**Option C: All Optional (Selected)** ✅
```yaml
# Simple case (old YAML)
task:
  path: "app.exe"

# Enhanced case (new YAML) - all optional additions
task:
  path: "app.exe"
  expectedReturnCode: 0
  assertions: []
  extractVariables: []
```
**Pros:** Single schema version, 100% backward compatible, simple parser, gradual adoption
**Cons:** Requires nil-checking in code, some properties contextually related (e.g., extractVariables only useful with redirectOutput)

### Decision Rationale
Option C chosen because:
1. **Backward compatibility:** Existing YAML configs work unchanged
2. **Single parser:** No version detection logic needed
3. **Gradual adoption:** Teams migrate at their own pace
4. **Simple implementation:** Optional properties are easier to implement than version branching

### Consequences
- ✅ Zero breaking changes
- ✅ User can adopt features incrementally
- ⚠️ Parser must handle nil values gracefully
- ⚠️ Documentation must clarify which properties are contextually related

---

## ADR-002: Dictionary<string, string> for Variable Flow

### Decision
Variables extracted from step output are stored in a simple `Dictionary<string, string>` and passed through the execution context to subsequent steps.

### Context
- test-framework extracts variables and passes them to next steps
- Need to flow variables in both sequential and parallel pipelines
- Variable substitution must work in arguments, paths, environment variables

### Options Considered

**Option A: Individual Variable Objects** ❌
```csharp
public class ExtractedVariable
{
    public string Name { get; set; }
    public string Value { get; set; }
    public int StepNumber { get; set; }
    public DateTime ExtractedAt { get; set; }
}

var variables = new List<ExtractedVariable>();
// Complex to manage, maintain history, validate
```
**Pros:** Rich metadata about each variable
**Cons:** Overcomplicated, difficult to substitute, ordering ambiguous

**Option B: Nested Dictionary by Step** ❌
```csharp
var variables = new Dictionary<string, Dictionary<string, string>>
{
    ["step1"] = new { "version" = "1.0.0" },
    ["step2"] = new { "branch" = "main" }
};
```
**Pros:** Variables scoped to step
**Cons:** Complex namespacing, collision possibility when merging

**Option C: Flat String Dictionary (Selected)** ✅
```csharp
var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["version"] = "1.0.0",
    ["branch"] = "main",
    ["commit"] = "abc123"
};

// Use in arguments: "app.exe --version $(version)"
```
**Pros:** Simple, testable, substitution straightforward, matches test-framework pattern
**Cons:** Global namespace (potential collisions), last value wins if duplicate

### Decision Rationale
Option C chosen because:
1. **Simplicity:** Single flat namespace easy to understand
2. **Test-framework alignment:** Matches existing proven approach
3. **Substitution:** Direct string replacement without parsing complexity
4. **Case-insensitive keys:** Support various naming conventions

### Consequences
- ✅ Simple variable substitution: `$(varname)`
- ✅ Works identically in sequential and parallel
- ✅ Testable: Just dictionary assertions
- ⚠️ Variable name collisions possible (last-write-wins)
- ⚠️ No scoping or shadowing possible
- ⚠️ Requires naming convention to avoid conflicts

---

## ADR-003: Regex-based Variable Extraction (Not Structured Parsing)

### Decision
Variables are extracted via regex patterns with capture groups, not structured parsers (JSON/XML).

### Context
- Executables output unstructured text (timestamps, progress messages, mixed formats)
- Need to support: JSON output, plain text, formatted tables, XML, binary with text headers
- test-framework uses regex patterns successfully

### Options Considered

**Option A: Structured Parsing** ❌
```yaml
extractVariables:
  - name: "version"
    type: "json"
    jsonPath: "$.version"
  - name: "timestamp"
    type: "xml"
    xmlPath: "/root/timestamp"
```
**Pros:** Type-safe, clear intent, supports structured formats
**Cons:** Requires parser for each format, complex code, doesn't handle mixed formats well

**Option B: Regex Pattern (Selected)** ✅
```yaml
extractVariables:
  - name: "version"
    pattern: '"version":\s*"([^"]+)"'  # For JSON
    groupIndex: 1
  
  - name: "timestamp"
    pattern: 'Timestamp: (\d{4}-\d{2}-\d{2})'  # For plain text
    groupIndex: 1
  
  - name: "branch"
    pattern: 'Branch: (\S+)'  # For any text containing "Branch: VALUE"
    groupIndex: 1
```
**Pros:** Universal (works for any text), flexible, proven in test-framework, minimal dependencies
**Cons:** Requires regex knowledge, harder to learn than declarative JSON path

**Option C: Custom Extractor Functions** ❌
```csharp
var extractors = new Dictionary<string, Func<string, string>>
{
    ["version"] = output => JsonDocument.Parse(output)
        .RootElement.GetProperty("version").GetString(),
    ["branch"] = output => Regex.Match(output, "Branch: (\\S+)").Groups[1].Value
};
```
**Pros:** Most flexible, supports any logic
**Cons:** Requires code, not declarative, hard to configure

### Decision Rationale
Option B chosen because:
1. **Universal:** Single approach works for all output formats
2. **Test-framework proven:** Already used successfully in production
3. **Declarative:** Defined in YAML, not code
4. **Simple:** One regex pattern per variable, easy to understand
5. **Flexible:** Can extract from JSON, XML, plain text, binary headers - anything with text

### Consequences
- ✅ Works for any executable output format
- ✅ No dependency on specific parser libraries
- ✅ Pattern validation at load time catches errors early
- ⚠️ Users must know regex syntax
- ⚠️ Complex patterns can be hard to debug
- 📋 **Mitigation:** Provide pattern examples library and validation helper

---

## ADR-004: Assertion Types Based on test-framework

### Decision
Assertion types are based on test-framework's proven set (output_contains, output_matches, etc.), not inventing new types.

### Context
- test-framework has successfully used assertions in production for years
- Need to reuse battle-tested validation logic
- Avoid reinventing wheels for common scenarios

### Options Considered

**Option A: Minimal Set** ❌
```yaml
assertions:
  - type: "success"        # Just pass/fail
```
**Pros:** Simple
**Cons:** Not expressive enough for real-world needs (need to validate output content)

**Option B: Extensive Custom Set** ❌
```yaml
assertions:
  - type: "output_contains"
  - type: "output_not_contains"
  - type: "output_matches"
  - type: "output_starts_with"
  - type: "output_ends_with"
  - type: "output_json_path"
  - type: "output_xml_path"
  - type: "exit_code"
  - type: "file_exists"
  - type: "file_size_gt"
  - type: "performance_under_ms"
  - type: "... (20+ custom types)"
```
**Pros:** Comprehensive coverage
**Cons:** Bloated, maintenance nightmare, hard to learn

**Option C: test-framework Proven Set (Selected)** ✅
```yaml
assertions:
  - type: "output_contains"
  - type: "output_not_contains"
  - type: "output_matches"
  - type: "output_equals"
  - type: "exit_code"
  - type: "file_exists"
  - type: "custom_json"       # Extensible via JSONPath
```
**Pros:** Proven, minimal, covers 80% of use cases, test-framework alignment
**Cons:** May not cover all edge cases (extensible via custom_json for advanced)

### Decision Rationale
Option C chosen because:
1. **Proven:** test-framework uses these in production
2. **Minimal:** Only 7 types covers most scenarios
3. **Extensible:** `custom_json` allows advanced validation
4. **Maintenance:** Easier to support small set
5. **Documentation:** Leverage existing test-framework docs

### Consequences
- ✅ Familiar to test-framework users
- ✅ Well-documented with examples
- ✅ Proven behavior and error messages
- ⚠️ Users requiring advanced assertions need `custom_json`
- ⚠️ Not expandable without code changes (design goal: keep YAML simple)

---

## ADR-005: No Pre/Post-Execution Hooks in Phase 1

### Decision
Pre/post-execution hooks (before task runs, after task completes) are planned for Phase 5, not Phase 1.

### Context
- Hooks would enable cleanup logic, notifications, monitoring integration
- Test-framework doesn't have hooks (not required for basic functionality)
- Adds complexity to Phase 1 (focus on core features)

### Options Considered

**Option A: Include Hooks in Phase 1** ❌
```yaml
task:
  preExecutionHook:
    command: "echo.exe"
    arguments: "Starting..."
  path: "app.exe"
  postExecutionHook:
    command: "echo.exe"
    arguments: "Done"
```
**Pros:** Complete feature set from day 1, enables cleanup patterns
**Cons:** Delays Phase 1 delivery, adds parser complexity, unclear hook semantics

**Option B: No Hooks (Selected for Phase 1)** ✅
**Phase 1:** Core execution with assertions/extraction
**Phase 5:** Add hooks after foundation proven

**Pros:** Focused Phase 1, faster delivery, test core features before adding complexity
**Cons:** Cleanup requires multiple tasks for now (acceptable workaround)

### Decision Rationale
Option B chosen because:
1. **Focused delivery:** Phase 1 delivers core value without hooks
2. **Test framework first:** Validate core design before adding complexity
3. **Workaround available:** Use `continueOnError: true` with separate cleanup tasks
4. **Iterative:** Add in Phase 5 once foundation proven

### Consequences
- ✅ Faster Phase 1 delivery (2 weeks vs. 3 weeks)
- ✅ Simpler initial design (easier to review)
- ✅ Foundation proven before adding hooks
- ⚠️ Cleanup requires explicit tasks in YAML (not implicit hooks)
- 📋 **Workaround:** Use this pattern for cleanup:
  ```yaml
  - name: "install"
    path: "installer.exe"
  
  - name: "cleanup"
    path: "cleanup.exe"
    dependencies: ["install"]
    continueOnError: true  # Always run, even if install fails
  ```

---

## ADR-006: Sequential Default, Not Implicit Parallel

### Decision
Default execution mode is sequential with explicit dependencies, not automatically parallel.

### Context
- Parallel is faster but riskier (ordering bugs, race conditions)
- Sequential is easier to debug and understand
- test-framework is sequential by default (step 1, 2, 3...)

### Options Considered

**Option A: Default Parallel** ❌
```yaml
# This would run all tasks in parallel by default
tasks:
  - path: "task1.exe"
  - path: "task2.exe"
  - path: "task3.exe"
```
**Pros:** Fast execution, naturally efficient
**Cons:** Ordering bugs hard to debug, variable extraction order unclear

**Option B: Default Sequential (Selected)** ✅
```yaml
# Default: sequential execution
tasks:
  - path: "task1.exe"
  - path: "task2.exe"      # Runs after task1 completes
  - path: "task3.exe"      # Runs after task2 completes

# Opt-in parallel with explicit mode
execution:
  mode: "parallel"
  maxConcurrency: 3
```
**Pros:** Predictable, test-framework aligned, variables flow clearly, easy to debug
**Cons:** Slower by default (user must opt-in to parallel)

### Decision Rationale
Option B chosen because:
1. **Debuggability:** Sequential ordering easier to trace
2. **Correctness:** Variable extraction order unambiguous
3. **test-framework alignment:** Steps numbered 1, 2, 3...
4. **Safety:** Dependencies clear, less room for mistakes
5. **Performance:** Users who need it can opt-in

### Consequences
- ✅ Predictable execution order
- ✅ Variables flow clearly between steps
- ✅ Familiar to test-framework users
- ⚠️ Slower by default (require `execution.mode: "parallel"`)
- ⚠️ Users must opt-in for parallel benefits
- 📋 **Rationale:** Speed vs. safety. Sequential safer, parallel available when needed.

---

## ADR-007: No Runtime Configuration Validation Errors

### Decision
Errors in YAML structure/values caught at **load time** (parsing), not runtime (execution).

### Context
- Syntax errors in YAML should fail fast during YamlLauncherConfigLoader
- Invalid regex patterns should fail during parsing
- Missing dependencies should fail during DependencyResolver (before execution)

### Options Considered

**Option A: Lenient Parsing** ❌
```csharp
// Load silently, error during execution
var config = loader.LoadFromFile("app.yaml");  // No errors
var result = await launcher.LaunchAsync(config);  // Error here ("pattern invalid")
```
**Pros:** Flexible, doesn't fail until needed
**Cons:** Errors discovered late, harder to debug, confusing user experience

**Option B: Strict Load-Time Validation (Selected)** ✅
```csharp
// Validate everything during load
var config = loader.LoadFromFile("app.yaml");  // Throws if invalid
// Regex patterns validated
// JSON structure validated
// Dependencies validated (DAG verified)
// All errors caught here, before execution

var result = await launcher.LaunchAsync(config);  // No validation errors here
```
**Pros:** Fast failure, clear error messages, fewer runtime surprises, better UX
**Cons:** Stricter parsing, must validate deeply

### Decision Rationale
Option B chosen because:
1. **Fast feedback:** Errors caught immediately
2. **Clear messages:** Load-time validation can be detailed
3. **Debugging:** User knows config is valid before execution
4. **User experience:** "Fix your YAML now" is better than "error during step 3"

### Consequences
- ✅ Errors caught before execution
- ✅ Clear validation error messages
- ✅ No surprises during long-running tasks
- ⚠️ Must validate deeply (regex compilation, JSON path syntax, etc.)
- ⚠️ Strict parsing might reject some edge cases

---

## ADR-008: No Retry Logic in Phase 1

### Decision
Retry policies (exponential backoff, max attempts) planned for Phase 5, not Phase 1.

### Context
- Retries complex (when to retry? which errors?)
- test-framework doesn't have retries
- Phase 1 focuses on core execution

### Options Considered

**Option A: Include Retries in Phase 1** ❌
```yaml
task:
  path: "flaky-service.exe"
  retryPolicy:
    maxAttempts: 3
    backoffMs: 1000
```
**Pros:** Handles flaky processes from day 1
**Cons:** Adds complexity, semantics unclear (retry on any error? only certain ones?)

**Option B: No Retries in Phase 1 (Selected)** ✅
**Phase 1:** Core execution, if task fails it fails
**Phase 5:** Add retry policies with clear semantics

**Pros:** Simpler Phase 1, clear when retries added
**Cons:** Flaky processes need manual handling for now

### Consequences
- ✅ Simpler Phase 1 design
- ✅ Faster delivery
- ⚠️ Users with flaky processes need workarounds
- 📋 **Workaround:** Run task multiple times with conditional logic
  ```yaml
  - name: "attempt1"
    path: "app.exe"
    continueOnError: true
  
  - name: "attempt2"
    path: "app.exe"
    dependencies: ["attempt1"]
    continueOnError: true
  ```

---

## ADR-009: String-based Value Comparison in Assertions

### Decision
Assertion values are always compared as strings, not parsed to specific types.

### Context
- Different output formats (JSON, CSV, plain text)
- Type coercion complex and error-prone
- String comparison universal

### Options Considered

**Option A: Type Inference** ❌
```yaml
assertions:
  - type: "exit_code"
    value: 0              # Integer?
  - type: "output_contains"
    value: "abc"          # String
  - type: "custom_json"
    expectedValue: true   # Boolean?
```
**Pros:** Semantically accurate types
**Cons:** Complex parsing, ambiguous (is "0" a number or string?), error-prone

**Option B: String Comparison (Selected)** ✅
```yaml
assertions:
  - type: "exit_code"
    value: "0"            # Compared as string
  - type: "output_contains"
    value: "abc"          # Compared as string
  - type: "custom_json"
    expectedValue: "true" # Compared as string
```
**Pros:** Consistent, simple, unambiguous, always works
**Cons:** Type mismatches harder to catch (0 vs "0" look same)

### Decision Rationale
Option B chosen because:
1. **Universal:** Works for all assertion types
2. **Simple:** Single comparison logic
3. **Safe:** No type coercion bugs
4. **Explicit:** YAML clearly shows what's compared

### Consequences
- ✅ Consistent behavior across all assertion types
- ✅ No type inference bugs
- ⚠️ Numbers compared as strings (0 == "0" but not 0 == "")
- 📋 **Best Practice:** Always use strings in YAML values

---

## ADR-010: Output Redirection Always On for Assertions

### Decision
When assertions are specified, output redirection (`redirectOutput: true`) is always enabled, regardless of property value.

### Context
- Assertions validate output content
- Can't validate output if not captured
- Default behavior should enable what's needed

### Options Considered

**Option A: Error if Assertions Without Redirection** ❌
```yaml
task:
  path: "app.exe"
  redirectOutput: false    # Error! Can't validate output
  assertions:
    - type: "output_contains"
      value: "success"
```
**Pros:** Forces user to be explicit
**Cons:** Confusing error message, extra step required

**Option B: Automatic Enable (Selected)** ✅
```yaml
task:
  path: "app.exe"
  # redirectOutput: false is ignored if assertions present
  assertions:
    - type: "output_contains"
      value: "success"
# Automatically: redirectOutput = true (implicitly)
```
**Pros:** Works as expected, no surprises, convenient
**Cons:** Magic behavior (implicit behavior not obvious)

### Decision Rationale
Option B chosen because:
1. **Convenience:** User doesn't need to remember both properties
2. **Works intuitively:** "I want to validate output" automatically captures it
3. **Less config:** One less thing to get wrong
4. **Documentation:** Clear in schema that assertions require output capture

### Consequences
- ✅ Assertions work intuitively
- ✅ One less configuration step
- ⚠️ Implicit behavior (might confuse some users)
- 📋 **Documentation:** Must clearly state this rule

---

## Summary Table: Key Tradeoffs

| Decision | Chosen | Alternative | Tradeoff |
|----------|--------|-------------|----------|
| Schema Properties | All Optional | Single Required | Backward Compatibility vs. Consistency |
| Variable Flow | Dictionary<string> | Object Model | Simplicity vs. Metadata |
| Variable Extraction | Regex Patterns | Structured Parsing | Flexibility vs. Learnability |
| Assertion Types | test-framework Set | Extended Set | Simplicity vs. Coverage |
| Execution Mode | Sequential Default | Parallel Default | Safety vs. Performance |
| Validation Timing | Load-Time | Runtime | Fast Feedback vs. Flexibility |
| Retry Logic | Phase 5 | Phase 1 | Focused Delivery vs. Feature Complete |
| Pre/Post Hooks | Phase 5 | Phase 1 | Focused Delivery vs. Feature Complete |
| Comparison Type | Strings | Type Inference | Simplicity vs. Type Safety |
| Output Redirect | Auto-On (assertions) | Manual Enable | Convenience vs. Explicitness |

---

## Review & Approval

**Architecture Decision Review:**
- [ ] Design approach sound
- [ ] Tradeoffs acceptable
- [ ] No critical gaps identified
- [ ] Ready for implementation

**Reviewers:**
- [ ] Architecture Lead - Date: ________
- [ ] Development Lead - Date: ________
- [ ] QA Lead - Date: ________

