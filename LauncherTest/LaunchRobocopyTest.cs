using Ntools;
using System.Diagnostics;

namespace LauncherTest
{
    public class LaunchRobocopyTest
    {
        public static ResultHelper Test()
        {
            var process = new Process();
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.FileName = "robocopy.exe";
            process.StartInfo.Arguments = "/?";
            process.StartInfo.RedirectStandardOutput = true;
            var result = process.LockStart(true);

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
            return result;
        }
    }
}
