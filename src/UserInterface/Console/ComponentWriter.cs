using System.Text;

namespace ApertureMessenger.UserInterface.Console;

/// <summary>
/// Handles writing CLI components to the console.
/// </summary>
public static class ComponentWriter
{
    public enum StepStates
    {
        Scheduled,
        Started,
        Completed
    }

    public static void WriteHeader(string title, ConsoleColor backgroundColor = ConsoleColor.White)
    {
        System.Console.BackgroundColor = backgroundColor;

        ConsoleWriter.Write(title, ConsoleColors.GetTextColorForBackground(backgroundColor));
        FillerWriter.WriteFiller(' ', System.Console.WindowWidth - title.Length);
        ConsoleWriter.WriteLine();

        System.Console.BackgroundColor = ConsoleColors.DefaultBackgroundColor;
    }

    public static void WriteUserInput(string prompt = ">", bool obfuscate = false)
    {
        ConsoleWriter.WriteLine();

        var commandResponseLength =
            $" {Shared.Response.GetTypeSymbol()} {Shared.Response.Response}".Length;
        var commandResponseBackgroundColor = Shared.Response.GetTypeConsoleColor();

        System.Console.BackgroundColor = commandResponseBackgroundColor;

        ConsoleWriter.MoveCursorToBottom(2);
        ConsoleWriter.Write(
            $" {Shared.Response.GetTypeSymbol()} {Shared.Response.Response}",
            ConsoleColors.GetTextColorForBackground(commandResponseBackgroundColor)
        );
        FillerWriter.WriteFiller(' ', System.Console.WindowWidth - commandResponseLength);
        ConsoleWriter.WriteLine();

        System.Console.BackgroundColor = ConsoleColors.DefaultBackgroundColor;
        ConsoleWriter.MoveCursorToBottom();
        ConsoleWriter.Write(prompt);

        var userInput = obfuscate ? ObfuscateString(Shared.UserInput) : Shared.UserInput;

        ConsoleWriter.WriteWithWordWrap($" {userInput}", firstLineOffset: prompt.Length);
    }

    public static void WriteStep(
        string description,
        int currentStage,
        int stepStage,
        int stepCompletedStage
    )
    {
        StepStates state;

        if (currentStage == stepStage)
            state = StepStates.Started;
        else if (currentStage >= stepCompletedStage)
            state = StepStates.Completed;
        else
            state = StepStates.Scheduled;

        var color = state switch
        {
            StepStates.Started => ConsoleColor.Cyan,
            StepStates.Completed => ConsoleColor.Green,
            _ => ConsoleColor.White
        };

        var prefix = state switch
        {
            StepStates.Started => "[\u2192]",
            StepStates.Completed => "[\u2713]",
            _ => "[ ]"
        };

        ConsoleWriter.WriteLine($"{prefix} {description}", color);
    }

    private static string ObfuscateString(string value, char obfuscator = '*')
    {
        var output = new StringBuilder();

        for (var i = 0; i < value.Length; i++) output.Append(obfuscator);

        return output.ToString();
    }
}