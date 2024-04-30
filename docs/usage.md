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
The Launcher class exposes `LockVerifyStart` method which is useful in scenarios where you need to start a process and verify that the file is digitally signed before launching. Here are some scenarios where you might use this method:

1. **Securely launching executables**: If you need to start an executable and want to ensure that it is digitally signed before launching, you can use this function to verify the digital signature of the file before starting the process.

2. **Preventing unauthorized executables**: In scenarios where you want to prevent unauthorized executables from running on your system, you can use this function to verify the digital signature of the file before launching it.


### NFile Class

The `NFile` class provides a method for downloading files from the web. Here's an example of how to use the `NFile` class:

```csharp
using Ntools;

 try
 {
    // It is assumed that the VirusTotal API key is stored in the environment variable
    // VTAPIKEY. The key is used to check the downloaded file for virus.
    var result = await NFile.DownloadAsync("https://example.com/file.zip", "C:\\temp\\file.zip");
    if (result.IsSuccess())
    {
        Console.WriteLine("Success");
    }
    else
    {
        Console.WriteLine($"Code: {result.Code}");
        Console.WriteLine($"Message: {result.Message}");
    }
 }
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```
The `DownloadAsync` method is useful in scenarios where you need to download a file from the web. Here are some scenarios where you might use this method:

1. **Downloading files from the web**: If you need to download a file from the web, you can use this function to do so.

2. **Checking for virus**: In scenarios where you want to check the downloaded file for viruses, you can use this function to download the file and then check it using a virus scanning service such as VirusTotal.

3. **Verifying file signature**: If you need to verify the digital signature of the downloaded file, you can use this function to do so.

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


**Boost your productivity with `ntools-launcher`**, an efficient library for automating common development tasks.

Streamline your development workflow with `ntools-launcher`. This versatile library simplifies a wide range of tasks, including:

* Launching executables directly from your code.
* Downloading files securily with ease.
* Executing shell commands for automated scripting.
* Checking process elevation for enhanced security awareness.

**Key benefits:**

* **Improved Efficiency:** Automate repetitive tasks and focus on core development activities.
* **Simplified Workflow:** User-friendly API streamlines integration into your projects.
* **Enhanced Control:** Manage processes and ensure proper elevation levels for secure development.

Start saving time and effort today! Install `ntools-launcher` via NuGet.

The library is used by the [ntools's](https://github.com/naz-hage/ntools) repo
- **Nbackup** launches `robocopy` and wait for it to complete, ensuring that the backup process is fully completed before the program continues.
- **Nbuild** launches `msbuild` to build and test .NET projects.  It also builds selected development tools for the project.  It utlilies the `NFile` class to download the tools from the web.
- **Ngit** launches `git` to perform git operations.