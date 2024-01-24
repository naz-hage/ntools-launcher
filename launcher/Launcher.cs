using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ntools
{
    public static class Launcher 
    {
        public static bool Verbose { get; set; } = false;

        public static ResultHelper LockVerifyStart(this Process process, bool verbose, bool lockVerify)
        {
            ResultHelper result;
            if (lockVerify)
            {
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
            }
            else
            {
                // Start the process.
                result = process.Start(verbose);
            }

            return result;
        }

        /// <summary>
        /// Starts the process and handles the output redirection or waiting for the process to exit.
        /// </summary>
        /// <param name="redirectStandardOutput">Whether or not to redirect any output from the executable to the result object's Output property.</param>
        /// <param name="verbose">Whether or not to output additional verbose messages.</param>
        /// <param name="process">The Process object representing the executable to launch.</param>
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
        /// Launch a process specified in parameters 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static ResultHelper Start(Parameters parameters)
        {
   
             var result = Start(
                                parameters.WorkingDir,
                                parameters.FileName,
                                parameters.Arguments,
                                parameters.RedirectStandardOutput,
                                parameters.Verbose,
                                parameters.UseShellExecute
                                );
           
            return result;
        }


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
        private static ResultHelper Start(string workingDir,
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
                                result = process.Start(verbose);
                            }

                        }
                    }
                    else
                    {
                        // Start the process.
                        result = process.Start(verbose);
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

        

    private static void RunProcessAsAdmin(string filename)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filename,
                UseShellExecute = true,
                Verb = "runas",
            }
        };

        process.Start();
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
                return ResultHelper.Fail(message:ex.Message);
            }
        }
    }
}
