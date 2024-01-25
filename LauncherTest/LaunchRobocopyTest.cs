using Ntools;

namespace LauncherTest
{
    public class LaunchRobocopyTest
    {
        public static ResultHelper Test()
        {
            var result = Ntools.Launcher.Start(
                new()
                {
                    WorkingDir = Directory.GetCurrentDirectory(),
                    Arguments = "/?",
                    FileName = "robocopy",
                    RedirectStandardOutput = true
                }
            );
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
