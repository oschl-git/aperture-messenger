using ApertureMessenger.UserInterface.Authentication.Views;
using ApertureMessenger.UserInterface.Console;
using ApertureMessenger.UserInterface.Interfaces;
using ApertureMessenger.UserInterface.Objects;

namespace ApertureMessenger.UserInterface.Authentication.Commands;

public class Exit : ICommand
{
    public string[] Aliases { get; } = ["exit", "quit", "e", "q"];

    public void Invoke(string[] args)
    {
        if (SharedData.View == AuthenticationView.GetInstance())
        {
            ConsoleWriter.Clear();
            Environment.Exit(0);
        }

        SharedData.CommandResponse = new CommandResponse(
            "Use the :login or :register commands to authenticate.",
            CommandResponse.ResponseType.Info
        );
        SharedData.View = AuthenticationView.GetInstance();
    }
}