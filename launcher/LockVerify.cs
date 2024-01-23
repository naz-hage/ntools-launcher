using System;
using System.IO;
using Launcher;

namespace Launcher
{
    public class LockVerify
    {
        // Method to start the lock verification process
        public ResultHelper Start(Parameters parameters, bool lockVerify)
        {
            var launcherBase = new LauncherBase();

            // Call the Start method of LauncherBase class with the provided parameters
            return launcherBase.Start(
                                parameters.WorkingDir,
                                parameters.FileName,
                                parameters.Arguments,
                                parameters.RedirectStandardOutput,
                                parameters.Verbose,
                                parameters.UseShellExecute,
                                lockVerify
                                );
        }
    }
}
