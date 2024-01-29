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

            // verify that working directory is path rooted and exists
            if (!Path.IsPathRooted(process.StartInfo.WorkingDirectory) ||
                !Directory.Exists(process.StartInfo.WorkingDirectory))
            {
                return ResultHelper.Fail(-1, $"Working directory '{process.StartInfo.WorkingDirectory}' is not path rooted");
            }

            // verify that file exists
            var executable = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
            if (!File.Exists(executable))
            {
                return ResultHelper.Fail(-1, $"File '{executable}' not found");
            }

            // Lock the file for reading
            using (FileStream fileStream = new FileStream(Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Check for valid digital signature
                if (!SignatureVerifier.VerifyDigitalSignature(fileStream.Name))
                {
                    result = ResultHelper.Fail(-1, $"File '{process.StartInfo.FileName}' is not digitally signed");
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
        /// Locks the file, and starts the process.
        /// </summary>
        /// <param name="process">The Process object representing the executable to launch.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        public static ResultHelper LockStart(this Process process, bool verbose)
        {
            ResultHelper result;

            // verify that working directory is path rooted and exists
            if (!Path.IsPathRooted(process.StartInfo.WorkingDirectory) ||
                !Directory.Exists(process.StartInfo.WorkingDirectory))
            {
                return ResultHelper.Fail(-1, $"Working directory {process.StartInfo.WorkingDirectory} is not path rooted");
            }

            // verify that file exists
            var executable = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
            if (!File.Exists(executable))
            {
                return ResultHelper.Fail(-1, $"File {executable} not found");
            }

            // Lock the file for reading
            using (FileStream fileStream = new FileStream(Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                    // Start the process.
                    result = process.Start(verbose);
            }


            return result;
        }
        /// <summary>
        /// Starts the process and handles the output redirection or waiting for the process to exit.
        /// </summary>
        /// <param name="process">The Process object representing the executable to launch.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        private static ResultHelper Start(this Process process, bool verbose)
        {
            var result = ResultHelper.New();

            // preserve current directory
            var currentDir = Directory.GetCurrentDirectory();
            
            try
            {
                // Set the working directory to the working directory.
                Directory.SetCurrentDirectory(process.StartInfo.WorkingDirectory);

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
                    result = ResultHelper.Fail(-1, $"Failed to start {process.StartInfo.FileName}");
                }

                // display exit code and process.output
                if (verbose)
                {
                    Console.WriteLine($" -Code: {process.ExitCode}");
                    Console.WriteLine($" -Output:");
                    foreach (var line in result.Output)
                    {
                        Console.WriteLine($"   {line}");
                    }
                }

                result.Code = process.ExitCode;
                if (result.Code != 0 && !process.StartInfo.RedirectStandardOutput)
                {
                    result.Output.Add($"ProcessStart Exception: {process.ExitCode}");
                }

                // restore current directory
                Directory.SetCurrentDirectory(currentDir);
            }
            catch (Exception ex)
            {
                result = ResultHelper.Fail(-1, $"Exception: {ex.Message}");
            }

            // check if current directory is restored
            if (Directory.GetCurrentDirectory() != currentDir)
            {
                if (result.Code == 0)
                {
                    result = ResultHelper.Fail(-1, $"Unable to restore current directory to {currentDir}");
                }
                else
                {
                    result.Output.Add($"Unable to restore current directory to {currentDir}");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Launches a process with the specified parameters.
        /// </summary>
        /// <param name="parameters">The Parameters object containing the launch parameters.</param>
        /// <returns>A ResultHelper object containing the exit code and, if redirectStandardOutput is true, any output from the executable.</returns>
        
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
