# ntools-launcher

Ntools is a .NET namespace that exposes classes:

- **Launcher:** A class with methods to launch a process and wait for it to complete.
    - **LockVerifyStart:** A methods that locks file and verify that a file is digitally signed before launching.
    - **LockStart:** A method that locks process before launching.
    - **LaunchInThread:** A method that launches a process in a separate thread.
- **ResultHelper:** Provides a helper class and methods to retrieve the result `Code` and `Output` of the launched executable.
- **ResultDownload:** Provides a helper class and methods to retrieve the result of the downloaded file.
- **CurrentProcess:** Provides a method to determine if the current process is elevated.
- **ShellUtility:** A helper class that executes shell commands.
    - **GetFullPathOfFile:** A method that retrieves the full path of a file from the Path environment variable.
- **NFile:** A class that downloads file from the web.
    - **DownloadAsync:** A method that downloads a file from the web and save to a local drive.

## ntools-launcher Nuget Package
The `ntools-launcher` package is available on nuget.org. To install the package, run the following command in the Package Manager Console:

```bash
Install-Package ntools-launcher
```

## Launcher Class Usage
To use the `ntools-launcher` library in your project, follow these steps:

1. Import the `Ntools` namespace in your code.
2. Create an instance of the `ResultHelper` class.
3. Use the static `LockStart` method to start the executable.
4. Retrieve the result code and output using the `Code` and `Output` properties of the `ResultHelper` instance.

Here's an example of how to use the `Launcher` class to start a process:

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

The `LockVerifyStart` method is useful in scenarios where you need to start a process and verify that the file is digitally signed before launching. Here are some scenarios where you might use this method:

1. **Securely launching executables**: If you need to start an executable and want to ensure that it is digitally signed before launching, you can use this function to verify the digital signature of the file before starting the process.

2. **Preventing unauthorized executables**: In scenarios where you want to prevent unauthorized executables from running on your system, you can use this function to verify the digital signature of the file before launching it.

## NFile Class Usage
The `NFile` class is used to download files from the web. To use the `NFile` class in your project, follow these steps:

1. Import the `Ntools` namespace in your code.
2. Create an instance of the `ResultDownload` class.
3. Use the `DownloadAsync` method to download the file from the web.
4. Retrieve the result of the download using the properties of the `ResultDownload` instance.

Here's an example of how to use the `NFile` class to download
a file from the web:

```csharp
using Ntools;

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
```

The `DownloadAsync` method is useful in scenarios where you need to download a file from the web. Here are some scenarios where you might use this method:

1. **Downloading files from the web**: If you need to download a file from the web, you can use this function to do so.

2. **Checking for virus**: In scenarios where you want to check the downloaded file for viruses, you can use this function to download the file and then check it using a virus scanning service such as VirusTotal.

3. **Verifying file signature**: If you need to verify the digital signature of the downloaded file, you can use this function to do so.

## ShellUtility Class Usage
The `ShellUtility` class is used to execute shell commands. To use the `ShellUtility` class in your project, follow these steps:

1. Import the `Ntools` namespace in your code.
2. Use the `GetFullPathOfFile` method to retrieve the full path of a file from the Path environment variable.

Here's an example of how to use the `ShellUtility` class to retrieve the full path of a file from the Path environment variable:

```csharp
using Ntools;

var fullPath = ShellUtility.GetFullPathOfFile("notepad.exe");
Console.WriteLine(fullPath);
```

The `GetFullPathOfFile` method is useful in scenarios where you need to retrieve the full path of a file from the Path environment variable. This can be useful when you need to execute a file using its full path.

## CurrentProcess Class Usage
The `CurrentProcess` class is used to determine if the current process is elevated. To use the `CurrentProcess` class in your project, follow these steps:

1. Import the `Ntools` namespace in your code.
2. Use the `IsElevated` method to determine if the current process is elevated.

Here's an example of how to use the `CurrentProcess` class to determine if the current process is elevated:

```csharp
using Ntools;

var isElevated = CurrentProcess.IsElevated();
Console.WriteLine(isElevated);
```

The `IsElevated` method is useful in scenarios where you need to determine if the current process is running with elevated privileges. This can be useful when you need to perform certain operations that require elevated privileges.

## ResultHelper Class Usage
The `ResultHelper` class is used to retrieve the result code and output of the launched executable. To use the `ResultHelper` class in your project, follow these steps:

1. Import the `Ntools` namespace in your code.
2. Create an instance of the `ResultHelper` class.
3. Use the `Code` and `Output` properties to retrieve the result code and output of the launched executable.

Here's an example of how to use the `ResultHelper` class to retrieve the result code and output of the launched executable:

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

The `ResultHelper` class is useful in scenarios where you need to retrieve the result code and output of the launched executable. This can be useful when you need to determine if the process completed successfully and retrieve any output that was generated.

## ResultDownload Class Usage
The `ResultDownload` class is used to retrieve the result of the downloaded file. To use the `ResultDownload` class in your project, follow these steps:

1. Import the `Ntools` namespace in your code.
2. Create an instance of the `ResultDownload` class.
3. Use the properties of the `ResultDownload` instance to retrieve the result of the download.

Here's an example of how to use the `ResultDownload` class to retrieve the result of the downloaded file:

```csharp
using Ntools;

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
```

The `ResultDownload` class is useful in scenarios where you need to retrieve the result of the downloaded file. This can be useful when you need to determine if the download was successful and retrieve any additional information about the download.


## LaunchInThread Usage
The `LaunchInThread` function is useful in scenarios where you need to start a process asynchronously without blocking the main thread of your application. Here are some scenarios where you might use this function:

1. **Running a long-running process**: If the process you're starting is expected to take a long time to complete, you might want to run it in a separate thread so that it doesn't block the main thread of your application. This is especially important in UI applications, where blocking the main thread can make the application unresponsive.

2. **Running multiple processes concurrently**: If you need to start multiple processes and have them run concurrently, you can do this by starting each one in a separate thread.

3. **Non-blocking operations**: In scenarios where you don't need to immediately know the result of the process, you can use this function to start the process and then continue with other tasks.

4. **Background tasks**: If the process you're starting is a background task that doesn't need to interact with the user or the main part of your application, you might choose to run it in a separate thread.

Remember that multithreading can make your code more complex and harder to debug, so it should be used judiciously. Always consider whether the benefits of running a process in a separate thread outweigh the added complexity.