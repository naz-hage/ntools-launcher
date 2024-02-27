using Ntools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Ntools
{
    /// <summary>
    /// This class provides utility methods related to File and Path operations
    /// </summary>
    public class ShellUtility
    {
        /// <summary>
        /// Retrieves the full path of a file from the Path environment variable.
        /// </summary>
        /// <param name="command">The command to search for the file.</param>
        /// <param name="versobe">Specifies whether to display verbose output.</param>
        /// <returns>The full path of the file if found, otherwise an empty string.</returns>
        public static string GetFullPathOfFile(string command, bool versobe = false)
        {
            if (string.IsNullOrEmpty(command)) return string.Empty;

            // Don't accept invalid characters
            //  < (less than)
            //	> (greater than)
            //	: (colon)
            //	" (double quote)
            //	/ (forward slash)
            //	\ (backslash)
            //	| (pipe)
            //	? (question mark)
            //	*(asterisk)
            // return empty if command contains invalid characters for directory or file name
            if (command.IndexOfAny(Path.GetInvalidPathChars()) >= 0) return string.Empty;

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.System)}",
                    FileName = "cmd.exe",
                    Arguments = $"/c where {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            var result = process.LockStart(versobe);
            if (!result.IsSuccess())
            {
                return string.Empty;
            }

            return result.GetFirstOutput();
        }
    }
}
