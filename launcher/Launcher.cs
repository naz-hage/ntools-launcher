using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Launcher
{
    public static class Launcher
    {
        public static bool Verbose { get; set; } = false;

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
                    bool useShellExecute = false)
        {
            var result = ResultHelper.New();
            Verbose = verbose;

            // Output verbose message if required.
            if (Verbose)
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

                    // Start the process.
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

                    // display exit code and process.output
                    if (Verbose)
                    {
                        Console.WriteLine($" -Output:");
                        foreach (var line in result.Output)
                        {
                            Console.WriteLine($"   {line}");
                        }

                        Console.WriteLine($" -Code: {process.ExitCode}");
                    }

                    

                    // Set the exit code in the result object and return it.
                    result.Code = process.ExitCode;
                }
                return result;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"ProcessStart Exception: {ex.Message}");
                result.Output.Add($"ProcessStart Exception: {ex.Message}");
                result.Code = int.MaxValue;
                return result;
            }
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

        private static void WriteError(string line)
        {
            if (line.ToLower().Contains("error") ||
                line.ToLower().Contains("fail") ||
                line.ToLower().Contains("rejected"))
            {
                Console.WriteLine(line);
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
                return ResultHelper.Fail(message:ex.Message);
            }
        }
    }
}
