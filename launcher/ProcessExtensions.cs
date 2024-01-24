using System;
using System.Diagnostics;
using System.IO;

namespace Ntools
{
    public static class ProcessExtensions
    {
        public static ResultHelper LockVerify(this Process process, bool verbose, bool lockVerify)
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
    }
}
