# ntools-launcher

Ntools is a .NET namespace that provides various utilities for launching processes. It includes the following class libraries:

- **Launcher:** Provides methods to launch a process and wait for it to complete.
    - **LockVerifyStart:** A methods that locks file and verify that a file is digitally signed before launching.
    - **LockStart:** A method that locks process before launching.
    - **LaunchInThread:** A method that launches a process in a separate thread.
- **ResultHelper:** Provides a helper class and methods to retrieve the result `Code` and `Output` of the launched executable.
- **CurrentProcess:** Provides a method to determine if the current process is elevated.
- **ShellUtility:** A helper class that executes shell commands.
    - **GetFullPathOfFile:** A method that retrieves the full path of a file from the Path environment variable.
- **Download:** A class that downloads file from the web.
    - **File:** A method that downloads a file from the web and save to a local drive.
    - **SignedFile:** A method that downloads a file and expects file to be digitally signed. If files is signed, method succeeds.  Otherwise, file is deleted.


## Installation

Provide instructions on how to install or add your library to a .NET project.

## Launcher Usage

## Usage
To use the `ntools-launcher` library in your project, follow these steps:

1. Install the `ntools-launcher` package from nuget.org.
2. Import the `Ntools` namespace in your code.
3. Create an instance of the `ResultHelper` class.
4. Use the `Start` method to execute the launcher executable.
5. Retrieve the result code and output using the `Code` and `Output` properties of the `ResultHelper` instance.

For more detailed information on how to use the launcher, refer to the [Launcher Usage](./launcher/README.md) documentation.

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
## LaunchInThread Usage
The `LaunchInThread` function is useful in scenarios where you need to start a process asynchronously without blocking the main thread of your application. Here are some scenarios where you might use this function:

1. **Running a long-running process**: If the process you're starting is expected to take a long time to complete, you might want to run it in a separate thread so that it doesn't block the main thread of your application. This is especially important in UI applications, where blocking the main thread can make the application unresponsive.

2. **Running multiple processes concurrently**: If you need to start multiple processes and have them run concurrently, you can do this by starting each one in a separate thread.

3. **Non-blocking operations**: In scenarios where you don't need to immediately know the result of the process, you can use this function to start the process and then continue with other tasks.

4. **Background tasks**: If the process you're starting is a background task that doesn't need to interact with the user or the main part of your application, you might choose to run it in a separate thread.

Remember that multithreading can make your code more complex and harder to debug, so it should be used judiciously. Always consider whether the benefits of running a process in a separate thread outweigh the added complexity.