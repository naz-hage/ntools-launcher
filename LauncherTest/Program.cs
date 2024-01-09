using LauncherTest;

try
{
    if (args.Length == 0)
    {
        Console.WriteLine("Invalid parameter: no parameter");
        Console.WriteLine($"Exit (-400)");
        Environment.Exit(-400);
    }

    switch (args[0].ToLower())
    {
        case "test":
            var result = LaunchRobocopyTest.Test();
            Console.WriteLine($"Exit ({result.Code})");
            Environment.Exit(result.Code);
            break;
        case "pass":
            Console.WriteLine($"pass");
            Console.WriteLine($"Exit (0)");
            Environment.Exit(0);
            break;
        case "fail":
            Console.WriteLine("fail");
            Console.WriteLine("error");
            Console.WriteLine("rejected");
            Console.WriteLine("exception");
            Console.WriteLine($"Exit (-100)");
            Environment.Exit(-100);
            break;

        default:
            Console.WriteLine($"Invalid parameter: {args[0]}");
            Console.WriteLine($"Exit (-300)");
            Environment.Exit(-300);
            break;
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
    Environment.Exit(-200);
}

