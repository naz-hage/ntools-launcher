using System;

namespace Launcher.Services;

/// <summary>
/// Provides consistent colored console output for launcher messages.
/// </summary>
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

    /// <summary>
    /// Writes a warning message to the console in yellow.
    /// </summary>
    /// <param name="message">The warning message to display.</param>
    public static void WriteWarning(string message)
    {
        WriteLine($"{message}", ConsoleColor.Yellow);
    }

    /// <summary>
    /// Writes a success message to the console in green with a checkmark prefix.
    /// </summary>
    /// <param name="message">The success message to display.</param>
    public static void WriteSuccess(string message)
    {
        // √ ✓
        WriteLine($"√ {message}", ConsoleColor.Green);
    }

    /// <summary>
    /// Writes a verbose diagnostic message to the console in gray.
    /// </summary>
    /// <param name="message">The diagnostic message to display.</param>
    public static void WriteVerbose(string message)
    {
        WriteLine($"{message}", ConsoleColor.Gray);
    }


    /// <summary>
    /// Writes an informational message to the console in cyan.
    /// </summary>
    /// <param name="message">The informational message to display.</param>
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
