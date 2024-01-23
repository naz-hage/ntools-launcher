# ntools-launcher

Ntools is a .NET namespace that provides various utilities for launching processes. It includes the following class libraries:

- **Launcher:** Provides methods to launch a process and wait for it to complete.
- **LockVerify:** Provides methods to verify that a file is digitally signed before launch.
- **ResultHelper:** Provides a helper class and methods to retrieve the result `Code` and `Output` of the launched executable.
- **CurrentProcess:** Provides a method to determine if the current process is elevated.

## Installation

Provide instructions on how to install or add your library to a .NET project.

## Usage

Here's an example of how to use the `Launcher` class to start a process:

```csharp
using Ntools;

var result = Launcher.Start(
    new()
    {
        WorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.System),
        Arguments = "/?",
        FileName = "robocopy.exe",
        RedirectStandardOutput = true
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
