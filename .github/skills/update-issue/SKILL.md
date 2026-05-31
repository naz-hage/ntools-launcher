---
name: update-issue
description: "Update issue/task status by checking acceptance criteria against code changes. Use when: user says 'update issue', 'update task', 'update issue 35', 'check acceptance criteria', 'mark issue done', 'complete issue'. User must provide issue number in format 'update issue 35'. Reads <number>-issue.md from .temp folder, validates acceptance criteria against current branch changes, updates status markers, adds next steps."
---

# Update Issue/Task Skill

## Overview

This skill automates the process of updating issue and task files with current implementation status. It:
1. Reads the issue file from `.temp/<number>-issue.md`
2. Checks acceptance criteria against actual code changes in the current branch
3. Marks completed items with `[x]`
4. Identifies incomplete items
5. Updates issue status (Done/Incomplete)
6. Provides next steps for remaining work

## Usage

User request format:
```
update issue 35
update task 36
update issue #40
```

## Workflow

### Step 1: Parse Issue Number
- Extract issue number from user request
- Construct file path: `.temp/<number>-issue.md`
- Validate file exists

### Step 2: Read Issue File
- Load the issue document
- Extract:
  - Issue title/description
  - Acceptance criteria (items listed as [ ] or [x])
  - Current status section
  - Any existing next steps

### Step 3: Analyze Code Changes
- Get list of files changed in current branch
- Identify relevant source files for this issue
- For each acceptance criterion:
  - Search for implementation evidence in modified files
  - Check for related test files and test coverage
  - Look for comments or code markers
  - Verify feature is complete and tested

### Step 4: Update Acceptance Criteria
- Mark completed items: `[x] Item description`
- Leave incomplete items: `[ ] Item description`
- Add supporting evidence (file path, line number) as comment

### Step 5: Update Status Section
- If all criteria complete: `**Status**: ✅ Done`
- If some incomplete: `**Status**: 🔄 Incomplete`
- Include timestamp

### Step 6: Add Next Steps
- If incomplete, list remaining work:
  - What acceptance criteria remain
  - What code changes are needed
  - Suggested order of work
- If complete, remove next steps or mark "None - ready for review"

### Step 7: Save Updated File
- Write changes back to `.temp/<number>-issue.md`
- Preserve formatting
- Report summary to user

## Example Transformation

**Before:**
```
## Acceptance Criteria
[ ] YamlLauncherConfigLoader class created
[ ] LoadFromFileAsync method implemented
[ ] LoadFromStringAsync method implemented
[ ] LoadFromStreamAsync method implemented
[ ] LauncherConfigValidator created
[ ] 11 unit tests passing
[ ] All tests passing (21/21)

**Status**: 🔄 In Progress

**Next Steps**
- Implement YamlLauncherConfigLoader
- Create unit tests
```

**After:**
```
## Acceptance Criteria
[x] YamlLauncherConfigLoader class created (launcher/YamlLauncher/YamlLauncherConfigLoader.cs)
[x] LoadFromFileAsync method implemented (line 45)
[x] LoadFromStringAsync method implemented (line 60)
[x] LoadFromStreamAsync method implemented (line 95)
[x] LauncherConfigValidator created (launcher/YamlLauncher/LauncherConfigValidator.cs)
[x] 11 unit tests passing (LauncherTests/YamlLauncher/YamlLauncherConfigLoaderTests.cs)
[x] All tests passing (21/21)

**Status**: ✅ Done

**Next Steps**
None - ready for review/merge
```

## Implementation Notes

- **Location**: Issue files must be in `.temp/` folder with pattern `<number>-issue.md`
- **File Format**: Markdown with standard sections (Acceptance Criteria, Status, Next Steps)
- **Evidence Tracking**: Include file paths and line numbers as comments for traceability
- **Test Verification**: Check for corresponding test files (.Tests.cs) to validate implementation
- **Branch Analysis**: Use `git diff main` or `git status` to identify changed files
- **Idempotent**: Safe to run multiple times; only updates what has changed

## When to Use

- After implementing features/fixes to track progress
- When reviewing code to validate acceptance criteria
- To generate status reports automatically
- Before creating pull requests to verify completeness
- During code review to track what was done

## When NOT to Use

- For creating new issues (use project management tool directly)
- For manual status tracking without code verification
- For issues without clear acceptance criteria
- For cross-repository issues
