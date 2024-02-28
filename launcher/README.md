# ntools-launcher

The `ntools-launcher` is a NuGet package library that simplifies common tasks related to launching executables, downloading files, executing shell commands, and checking process elevation. This library is designed to be easy to use, while providing robust functionality for a variety of tasks.

## Features

- **Launcher:** A class with methods to launch a process and wait for it to complete. This includes methods for locking a file and verifying its digital signature before launching, and launching a process in a separate thread.
- **ResultHelper:** A helper class for retrieving the result `Code` and `Output` of the launched executable.

- **NFile:** A class with a method for downloading files from the web. This method for checks Web download Url inegrity, the downloaded file signature, file size, and VirusTotal check.
- **ResultDownload:** A helper class for retrieving the result of a downloaded file.

- **CurrentProcess:** A class that provides a method to determine if the current process is elevated.

- **ShellUtility:** A helper class for executing shell commands and retrieving the full path of a file from the Path environment variable.

## Installation

The `ntools-launcher` package is available on nuget.org. To install the package, run the following command in the Package Manager Console:

```bash
Install-Package ntools-launcher
```

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

## Contributing

Contributions are welcome! Please read our [contributing guidelines](CONTRIBUTING.md) for details on how to contribute to this project.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.