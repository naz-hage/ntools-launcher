# ntools-launcher

Ntools is a .NET namespace that provides various utilities for launching processes. It includes the following class libraries:

- **Launcher:** Provides methods to launch a process and wait for it to complete.
- **LockVerify:** Provides methods to verify that a file is digitally signed before launch.
- **ResultHelper:** Provides a helper class and methods to retrieve the result `Code` and `Output` of the launched executable.
- **CurrentProcess:** Provides a method to determine if the current process is elevated.

## Installation

Provide instructions on how to install or add your library to a .NET project.

## Launcher Usage

Here's an example of how to use the `Launcher` class to start a process:

```csharp
using Ntools;

var result = Launcher.Start(
    new()
    {
        WorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.System),
        Arguments = "/?",
        FileName = "robocopy.exe",
        RedirectStandardOutput = true,
        RedirectStandardError = true
    }
);

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