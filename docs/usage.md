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
The `DownloadAsync` method downloads files and checks for viruses using VirusTotal, with optional digital signature verification.

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