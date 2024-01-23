using System;
using System.Diagnostics;
using System.IO;

namespace Launcher
{
    public class LauncherBase
    {

        /// <summary>
        /// A wrapper to launch an executable with arguments. The executable must run silently without requiring user input.
        /// If redirectStandardOutput is true, any output from the executable will be redirected to the result object's Output property.
        /// </summary>
        /// <param name="workingDir">The working directory to run the executable from.</param>
        /// <param name="fileName">The file name (including extension) of the executable to launch.</param>
        /// <param name="arguments">The command line arguments to pass to the executable.</param>
        /// <param name="redirectStandardOutput">Whether or not to redirect any output from the executable to the result object's Output property.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <param name="useShellExecute">Whether or not to use the system shell to start the process. This should be false to allow I/O redirection.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        public ResultHelper Start(string workingDir,
                    string fileName,
                    string arguments,
                    bool redirectStandardOutput = false,
                    bool verbose = false,
                    bool useShellExecute = false,
                    bool lockVerify = false)
        {
            var result = ResultHelper.New();

            // preserve current directory
            var currentDir = Directory.GetCurrentDirectory();
            if (verbose) Console.WriteLine($"CurrentDir: {currentDir}");

            // Output verbose message if required.
            if (verbose)
            {
                Console.WriteLine($" -Launcher   => {fileName} {arguments}");
                Console.WriteLine($" -WorkingDir => {workingDir}");
            }

            try
            {
                // Create a new process object and set the properties for running the specified executable.
                using (var process = new Process())
                {
                    process.StartInfo.WorkingDirectory = workingDir;
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.UseShellExecute = useShellExecute;
                    process.StartInfo.RedirectStandardOutput = redirectStandardOutput;
                    process.StartInfo.RedirectStandardError = redirectStandardOutput;

                    // Set the current directory to the working directory to avoid issues when running the executable.
                    Directory.SetCurrentDirectory(process.StartInfo.WorkingDirectory);

                    if (lockVerify)
                    {
                        // Lock the file for reading
                        using (FileStream fileStream = new FileStream(Path.Combine(workingDir, fileName), FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            // Check for valid digital signature
                            if (!SignatureVerifier.VerifyDigitalSignature(fileStream.Name))
                            {
                                result = ResultHelper.Fail(-1, $"File {fileName} is not digitally signed");
                            }
                            else
                            {
                                // Start the process.
                                result = Start(redirectStandardOutput, verbose, process);
                            }

                        }
                    }
                    else
                    {
                        // Start the process.
                        result = Start(redirectStandardOutput, verbose, process);
                    }
                    // File is automatically unlocked here as 'using' block is exited and FileStream is disposed
                    // Set the exit code in the result object and return it.
                }
            }

            catch (Exception ex)
            {
                if (verbose) Console.WriteLine($"ProcessStart Exception: {ex.Message}");
                result = ResultHelper.Fail(-1, $"File {fileName} not found");
            }

            // restore current directory
            Directory.SetCurrentDirectory(currentDir);
            if (verbose) Console.WriteLine($"reset CurrentDir: {Directory.GetCurrentDirectory()}");

            return result;
        }

        /// <summary>
        /// Starts the process and handles the output redirection or waiting for the process to exit.
        /// </summary>
        /// <param name="redirectStandardOutput">Whether or not to redirect any output from the executable to the result object's Output property.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <param name="process">The Process object representing the executable to launch.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        private static ResultHelper Start(bool redirectStandardOutput, bool verbose, Process process)
        {
            var result = ResultHelper.New();

            if (process.Start())
            {
                // If redirectStandardOutput is true, read any output from the executable and add it to the result object's Output property.
                if (redirectStandardOutput)
                {
                    result.Output.AddRange(process.StandardOutput.ReadToEnd()
                                    .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
                    result.Output.AddRange(process.StandardError.ReadToEnd()
                                   .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else // Otherwise, wait for the process to exit.
                {
                    process.WaitForExit();

                }
            }
            else
            {
                return ResultHelper.Fail(-1, $"File {process.StartInfo.FileName} not found");
            }

            // display exit code and process.output
            if (verbose)
            {
                Console.WriteLine($" -Output:");
                foreach (var line in result.Output)
                {
                    Console.WriteLine($"   {line}");
                }

                Console.WriteLine($" -Code: {process.ExitCode}");
            }

            result.Code = process.ExitCode;
            if (result.Code != 0 && !redirectStandardOutput)
            {
                result.Output.Add($"ProcessStart Exception: {process.ExitCode}");
            }

            return result;
        }
    }
}