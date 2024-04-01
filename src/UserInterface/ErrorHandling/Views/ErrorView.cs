using ApertureMessenger.AlmsConnection.Exceptions;
using ApertureMessenger.UserInterface.Console;
using ApertureMessenger.UserInterface.ErrorHandling.Commands;
using ApertureMessenger.UserInterface.Interfaces;
using ApertureMessenger.UserInterface.Objects;

namespace ApertureMessenger.UserInterface.ErrorHandling.Views;

public class ErrorView : IView
{
    private static readonly ICommand[] Commands =
    [
        new Retry(),
        new Exit()
    ];

    private Exception _exception;

    public ErrorView(Exception exception)
    {
        _exception = exception;
    }

    public void Process()
    {
        Shared.View = this;
        Shared.CommandResponse = new CommandResponse("Use :retry to restart the application or :exit to exit.",
            CommandResponse.ResponseType.Info);

        while (true)
        {
            Shared.RefreshView();

            var userInput = ConsoleReader.ReadCommandFromUser();
            var commandResult = CommandProcessor.InvokeCommand(userInput, Commands);

            switch (commandResult)
            {
                case CommandProcessor.Result.NotACommand:
                    Shared.CommandResponse = new CommandResponse(
                        "Commands must start with a colon (:).",
                        CommandResponse.ResponseType.Error
                    );
                    break;
                case CommandProcessor.Result.InvalidCommand:
                    Shared.CommandResponse = new CommandResponse(
                        $"{userInput} is not a valid command in this context.",
                        CommandResponse.ResponseType.Error
                    );
                    break;
                case CommandProcessor.Result.Success:
                default:
                    return;
            }
        }
    }

    public void DrawUserInterface()
    {
        ConsoleWriter.Clear();

        ComponentWriter.WriteHeader("FATAL RUNTIME ERROR", ConsoleColor.DarkRed);
        ConsoleWriter.WriteLine();

        ConsoleWriter.WriteWithWordWrap(
            $"\u26a0 {GetMessage()}", ConsoleColor.Red
        );
        ConsoleWriter.WriteLine();

        ConsoleWriter.WriteWithWordWrap(
            "If this is unexpected behaviour, please create an issue with the details below at " +
            "github.com/oschl-git/aperture-messenger. Your help is greatly appreciated!"
        );
        ConsoleWriter.WriteLine();

        ConsoleWriter.WriteLine();
        ConsoleWriter.WriteLine();
        ConsoleWriter.WriteLine("DETAILS:", ConsoleColor.Yellow);
        ConsoleWriter.WriteWithWordWrap(_exception.ToString(), ConsoleColor.Red);

        ComponentWriter.WriteUserInput();
    }

    private string GetMessage()
    {
        return _exception switch
        {
            FailedContactingAlms => "Failed contacting ALMS. Check your internet connection.",
            _ => "An unexpected fatal error has occurred during the runtime of Aperture Messenger."
        };
    }
}