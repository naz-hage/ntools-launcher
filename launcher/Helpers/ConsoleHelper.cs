using System;

namespace Launcher.Helpers;

public static class ConsoleHelper
{
    /// <summary>
    /// Writes a message to the console with an optional color.
    /// </summary>
    /// <param name="message">The message to write to the console.</param>
    /// <param name="color">The optional color for the message text.</param>
    public static void WriteLine(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
        {
            Console.ForegroundColor = color.Value;
        }

        Console.WriteLine(message);

        // Reset the console color to the default
        if (color.HasValue)
        {
            Console.ResetColor();
        }
    }

    
    /// <summary>
    /// Writes an error message to the console with a prefix X indicating it's an error message.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void WriteError(string message)
    {
        WriteLine($"X {message}", ConsoleColor.Red);
    }

    public static void WriteWarning(string message)
    {
        WriteLine($"{message}", ConsoleColor.Yellow);
    }

    public static void WriteSuccess(string message)
    {
        // √ ✓
        WriteLine($"√ {message}", ConsoleColor.Green);
    }

    public static void WriteVerbose(string message)
    {
        WriteLine($"{message}", ConsoleColor.Gray);
    }


    public static void WriteInfo(string message)
    {
        WriteLine($"{message}", ConsoleColor.Cyan);
    }

    /// <summary>
    /// Displays a solid line in the console to separate sections of output.
    /// </summary>
    internal static void SolidLine()
    {
        WriteLine(new string('─', 40));
    }
}
