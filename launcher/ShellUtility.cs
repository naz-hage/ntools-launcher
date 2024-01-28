using Ntools;
using System;
using System.Diagnostics;

namespace Ntools
{
    // This class provides utility methods related to File and Path operations
    public class ShellUtility
    {
        // Retrieves the full path of a file from the Path environment variable
        // Parameters:
        //   command: The command to search for the file
        // Returns:
        //   The full path of the file if found, otherwise an empty string
        public static string GetFullPathOfFile(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.System)}",
                    FileName = "cmd.exe",
                    Arguments = $"/c where {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };
            var result = process.LockStart(false);

            return result.GetFirstOutput();
        }
    }
}
