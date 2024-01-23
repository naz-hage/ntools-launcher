# 
- **launcher** - A .NET class library which exposes the launcher class and methods to launch a process and wait for it to complete..

- example usage:

```c#
using Launcher
var result = Launcher.Launcher.Start(
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
