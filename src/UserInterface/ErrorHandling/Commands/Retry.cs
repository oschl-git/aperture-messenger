using ApertureMessenger.UserInterface.Authentication.Views;
using ApertureMessenger.UserInterface.Interfaces;

namespace ApertureMessenger.UserInterface.ErrorHandling.Commands;

/// <summary>
/// An error handling command that handles restarting the application.
/// </summary>
public class Retry : IActionCommand
{
    public string[] Aliases { get; } = ["retry", "restart", "continue", "r", "c"];
    public string Description => "Restarts the application.";
    public Tuple<string, string>[] Arguments { get; } = [];

    public void Invoke(string[] args)
    {
        Shared.View = new ConnectionView();
    }
}