## Usage

### Launcher Class

The `Launcher` class provides methods for launching a process. This includes methods for locking a file and verifying its digital signature before launching, and launching a process in a separate thread.

Here's an example of how to use the `Launcher` class:

```csharp
using Ntools;

var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System),
        FileName = "robocopy.exe",
        Arguments = "/?",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        CreateNoWindow = false,
        UseShellExecute = false
    }
};

var result = process.LockStart(true);
if (result.IsSuccess())
{
    Console.WriteLine("Success");
}
else
{
    Console.WriteLine($"Code: {result.Code}");
    foreach (var line in result.Output)
    {
        Console.WriteLine(line);
    }
}
```
The `LockVerifyStart` method verifies digital signatures before launching processes, useful for secure executable launching and preventing unauthorized execution.


### Nfile Class

The `Nfile` class downloads a file from a trusted HTTPS host and validates the
allowed extension. After a successful download, `ResultDownload` records the
file size and whether the downloaded file has a digital signature.

```csharp
using Ntools;

 try
 {
    Nfile.SetTrustedHosts(new List<string> { "example.com" });
    Nfile.SetAllowedExtensions(new List<string> { ".zip" });
    var result = await Nfile.DownloadAsync("https://example.com/file.zip", "C:\\temp\\file.zip");
    if (result.IsSuccess())
    {
        Console.WriteLine("Success");
    }
    else
    {
        Console.WriteLine($"Code: {result.Code}");
        Console.WriteLine(result.GetFirstOutput());
    }
 }
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```
`DownloadAsync` uses the configured `HttpClient` certificate validation callback;
an HTTP status such as 404 is returned as a download failure and is not treated
as a certificate error. VirusTotal scanning is not automatic; use
`VirusTotalChecker` explicitly when that integration is required.

### ShellUtility Class

The `ShellUtility` class provides a method for retrieving the full path of a file from the Path environment variable. Here's an example of how to use the `ShellUtility` class:

```csharp
using Ntools;

var fullPath = ShellUtility.GetFullPathOfFile("notepad.exe");
Console.WriteLine(fullPath);
```

### CurrentProcess Class

The `CurrentProcess` class provides a method to determine if the current process is elevated. Here's an example of how to use the `CurrentProcess` class:

```csharp
using Ntools;

var isElevated = CurrentProcess.IsElevated();
Console.WriteLine(isElevated);
```

### YAML Launcher Framework

The YAML Launcher framework loads a validated `LauncherConfig` and executes configured steps. Use `steps:` as the canonical YAML property; `tasks:` and `apps:` are supported aliases that map to the same collection.

#### Minimal Runnable Configuration

This is the smallest useful configuration. It runs `dotnet --version` and can be loaded with the code below.

```yaml
version: "1.0"
description: "Check the installed .NET version"
execution:
    verbose: true
steps:
    - name: dotnet-version
        path: dotnet
        arguments: "--version"
        expectedReturnCode: 0
```

Load, validate, and execute the first step:

```csharp
using YamlLauncher;
using YamlLauncher.Models;

var loader = new YamlLauncherConfigLoader();
LauncherConfig config = await loader.LoadFromFileAsync("launcher.yaml");

var executor = new StepExecutor(config.Execution?.Verbose ?? false);
LaunchResult result = await executor.LaunchAsync(config, stepIndex: 0);

var execution = result.Results[0];
Console.WriteLine($"Exit code: {execution.ExitCode}");
Console.WriteLine(execution.StandardOutput);
```

#### Full Configuration Reference

The following example shows every supported configuration field. Values such as `workingDirectory` and `requireSignature` must be adapted to the local environment before use.

```yaml
version: "1.0" # required; format: X.Y or X.Y.Z
description: "Build the application" # optional; default: null
variables: # optional; default: null; global environment variables
    Configuration: "Release"
execution: # optional; defaults apply when omitted
    mode: Sequential # optional; values: Sequential, Parallel; default: Sequential
    verbose: true # optional; values: true, false; default: false
    stopOnFirstError: true # optional; values: true, false; default: false
    maxConcurrency: 4 # optional; positive integer; default: processor count; used in Parallel mode
    timeout: 60 # optional; non-negative integer seconds; default: 0 (no timeout)
steps: # required; must contain at least one step; tasks and apps are aliases
    - name: build # required; step identifier
        path: dotnet # required; executable or script path
        arguments: "build -c $(Configuration)" # optional; default: empty
        dependencies: [] # optional; default: null; names of prerequisite steps
        continueOnError: false # optional; true or false; default: false
        expectedReturnCode: 0 # optional; non-negative integer; default: 0
        workingDirectory: null # optional; default: process working directory
        environment: # optional; default: null; step-specific environment variables
            BUILD_NUMBER: "42"
        requireElevation: false # optional; true or false; default: false
        requireSignature: null # optional; default: null; certificate subject on Windows
        assertions: # optional; default: null
            - type: exit_code # required; values: exit_code, output_contains, output_matches
                value: "0" # required for exit_code and output_contains
                pattern: null # required for output_matches; valid regular expression
                jsonPath: null # reserved model field; not currently evaluated
                expectedValue: null # reserved model field; not currently evaluated
                caseInsensitive: false # optional; true or false; default: false
                description: "The build must succeed" # optional; default: null
        extractVariables: # optional; default: null
            - name: BuildVersion # required; extracted variable name
                pattern: "Version: ([0-9.]+)" # required; valid regular expression
                groupIndex: 1 # optional; non-negative integer; default: 0 (full match)
                caseInsensitive: false # optional; true or false; default: false
                description: "Version reported by the build" # optional; default: null
```

`LoadFromStringAsync` and `LoadFromStreamAsync` are also available. Invalid YAML, missing required configuration, dangling dependencies, and invalid regular expressions are reported through `LauncherConfigException`; YAML syntax errors include line and column information.

`StepExecutor` captures standard output and standard error, applies global and
step-specific environment variables, validates an optional working directory,
enforces `execution.timeout` in seconds, and returns exit code `-1` when
execution times out or cannot start. `requireSignature` is an optional
certificate subject requirement for the executable. Verbose logging uses
`[LAUNCHER]` prefixes and child processes run without creating a console
window.

The current framework executes individual steps. Dependency orchestration, variable substitution between steps, and full assertion evaluation remain planned extensions.