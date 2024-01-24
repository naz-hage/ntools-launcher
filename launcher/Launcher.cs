using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ntools
{
    public static class Launcher 
    {
        /// <summary>
        /// Locks the file, verifies the digital signature, and starts the process.
        /// </summary>
        /// <param name="process">The Process object representing the executable to launch.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        public static ResultHelper LockVerifyStart(this Process process, bool verbose)
        {
            ResultHelper result;
            // Lock the file for reading
            using (FileStream fileStream = new FileStream(Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Check for valid digital signature
                if (!SignatureVerifier.VerifyDigitalSignature(fileStream.Name))
                {
                    result = ResultHelper.Fail(-1, $"File {process.StartInfo.FileName} is not digitally signed");
                }
                else
                {
                    // Start the process.
                    result = process.Start(verbose);
                }
            }

            return result;
        }

        /// <summary>
        /// Starts the process and handles the output redirection or waiting for the process to exit.
        /// </summary>
        /// <param name="process">The Process object representing the executable to launch.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        public static ResultHelper Start(this Process process, bool verbose)
        {
            var result = ResultHelper.New();

            if (process.Start())
            {
                // If redirectStandardOutput is true, read any output from the executable and add it to the result object's Output property.
                if (process.StartInfo.RedirectStandardOutput && process.StartInfo.RedirectStandardError)
                {
                    result.Output.AddRange(process.StandardOutput.ReadToEnd()
                                    .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
                    result.Output.AddRange(process.StandardError.ReadToEnd()
                                   .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    // If redirectStandardOutput is false, wait for the process to exit and add the exit code to the result object's Code property.
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
            if (result.Code != 0 && !process.StartInfo.RedirectStandardOutput)
            {
                result.Output.Add($"ProcessStart Exception: {process.ExitCode}");
            }

            return result;
        }

        /// <summary>
        /// Launches a process with the specified parameters.
        /// </summary>
        /// <param name="parameters">The Parameters object containing the launch parameters.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        public static ResultHelper Start(Parameters parameters)
        {
            var result = ResultHelper.New();

            // preserve current directory
            var currentDir = Directory.GetCurrentDirectory();
            if (parameters.Verbose) Console.WriteLine($"CurrentDir: {currentDir}");

            // Output verbose message if required.
            if (parameters.Verbose)
            {
                Console.WriteLine($" -Launcher   => {parameters.FileName} {parameters.Arguments}");
                Console.WriteLine($" -WorkingDir => {parameters.WorkingDir}");
            }

            try
            {
                // Create a new process object and set the properties for running the specified executable.
                using (var process = new Process())
                {
                    process.StartInfo.WorkingDirectory = parameters.WorkingDir;
                    process.StartInfo.FileName = parameters.FileName;
                    process.StartInfo.Arguments = parameters.Arguments;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.UseShellExecute = parameters.UseShellExecute;
                    process.StartInfo.RedirectStandardOutput = parameters.RedirectStandardOutput;
                    process.StartInfo.RedirectStandardError = parameters.RedirectStandardError;

                    // Set the current directory to the working directory to avoid issues when running the executable.
                    Directory.SetCurrentDirectory(process.StartInfo.WorkingDirectory);

                    result = process.Start(parameters.Verbose);
                }
            }
            catch (Exception ex)
            {
                if (parameters.Verbose) Console.WriteLine($"ProcessStart Exception: {ex.Message}");
                result = ResultHelper.Fail(-1, $"File {parameters.FileName} not found");
            }

            // restore current directory
            Directory.SetCurrentDirectory(currentDir);
            if (parameters.Verbose) Console.WriteLine($"reset CurrentDir: {Directory.GetCurrentDirectory()}");

            return result;
        }

        /// <summary>
        /// Launches an executable with arguments.
        /// </summary>
        /// <param name="workingDir">The working directory to run the executable from.</param>
        /// <param name="fileName">The file name (including extension) of the executable to launch.</param>
        /// <param name="arguments">The command line arguments to pass to the executable.</param>
        /// <param name="redirectStandardOutput">Whether or not to redirect any output from the executable to the result object's Output property.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <param name="useShellExecute">Whether or not to use the system shell to start the process. This should be false to allow I/O redirection.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        public static ResultHelper LaunchExecutable(string workingDir, string fileName, string arguments, bool redirectStandardOutput, bool verbose, bool useShellExecute)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDir,
                Arguments = arguments,
                UseShellExecute = useShellExecute,
                RedirectStandardOutput = redirectStandardOutput,
                CreateNoWindow = true,
            };

            try
            {
                var executable = Path.Combine(workingDir, fileName);
                if (!File.Exists(executable))
                {
                    return ResultHelper.Fail(message: $"File {executable} not found");
                }
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Process.Start(startInfo);
                        Console.WriteLine($"Started {startInfo.FileName} {startInfo.Arguments}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }).Start();

                return ResultHelper.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return ResultHelper.Fail(message: ex.Message);
            }
        }

        /// <summary>
        /// Launch in thread and exit
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="workingDir"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static ResultHelper LaunchInThread(string workingDir, string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDir,
                Arguments = arguments,
                UseShellExecute = true,
                RedirectStandardOutput = false,
                CreateNoWindow = true,
            };

            try
            {
                var executable = $"{workingDir}\\{fileName}";
                if (!File.Exists(executable))
                {
                    return ResultHelper.Fail(message: $"File {executable} not found");
                }
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Process.Start(startInfo);
                        Console.WriteLine($"Started {startInfo.FileName} {startInfo.Arguments}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }).Start();

                return ResultHelper.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return ResultHelper.Fail(message: ex.Message);
            }
        }
    }
}
