using ApertureMessenger.AlmsConnection;
using ApertureMessenger.AlmsConnection.Authentication;
using ApertureMessenger.AlmsConnection.Repositories;
using ApertureMessenger.AlmsConnection.Requests;
using ApertureMessenger.UserInterface.Authentication.Commands;
using ApertureMessenger.UserInterface.Console;
using ApertureMessenger.UserInterface.Interfaces;
using ApertureMessenger.UserInterface.Messaging.Views;
using ApertureMessenger.UserInterface.Objects;

namespace ApertureMessenger.UserInterface.Authentication.Views;

/// <summary>
/// A view/UI handler for displaying the login CLI.
/// </summary>
public class LoginView : IView
{
    private enum Stage
    {
        UsernameInput,
        UsernameVerification,
        PasswordInput,
        LoginAttempt,
        LoginSuccess,
        LoginAborted
    }

    private Stage _currentStage = Stage.UsernameInput;

    private string? _submittedUsername;
    private string? _submittedPassword;

    public void Process()
    {
        Shared.Feedback = new CommandFeedback(
            "Input your authentication details to log in.",
            CommandFeedback.FeedbackType.Info
        );

        while (_currentStage != Stage.LoginSuccess)
        {
            Shared.RefreshView();

            switch (_currentStage)
            {
                case Stage.UsernameInput:
                    HandleUsernameInput();
                    break;
                case Stage.UsernameVerification:
                    HandleUsernameVerification();
                    break;
                case Stage.PasswordInput:
                    HandlePasswordInput();
                    break;
                case Stage.LoginAttempt:
                    HandleLoginAttempt();
                    break;
                case Stage.LoginSuccess:
                case Stage.LoginAborted:
                default:
                    return;
            }
        }
    }

    public void DrawUserInterface()
    {
        ConsoleWriter.Clear();

        ComponentWriter.WriteHeader("ALMS EMPLOYEE LOGIN", ConsoleColor.DarkCyan);
        ConsoleWriter.WriteLine();

        ConsoleWriter.WriteWithWordWrap(
            "To finish authenticating, complete the following steps:",
            ConsoleColor.Yellow
        );
        ConsoleWriter.WriteLine();
        ConsoleWriter.WriteLine();

        ComponentWriter.WriteStep(
            "Input a username.",
            (int)_currentStage,
            (int)Stage.UsernameInput,
            (int)Stage.PasswordInput
        );
        ComponentWriter.WriteStep(
            "Input a password.",
            (int)_currentStage,
            (int)Stage.PasswordInput,
            (int)Stage.LoginSuccess
        );

        ConsoleWriter.WriteLine();
        ConsoleWriter.WriteWithWordWrap("Use the :exit command to cancel logging in.", ConsoleColor.Red);

        ComponentWriter.WriteUserInput(GetPrompt(), _currentStage == Stage.PasswordInput);
    }

    private void HandleUsernameInput()
    {
        _submittedUsername = ConsoleReader.ReadCommandFromUser();
        if (CheckForExitCommand(_submittedUsername)) return;
        _currentStage = Stage.UsernameVerification;
    }

    private void HandleUsernameVerification()
    {
        Shared.Feedback = new CommandFeedback(
            "Checking username validity...",
            CommandFeedback.FeedbackType.Loading
        );
        Shared.RefreshView();

        var usernameExists = _submittedUsername != null && EmployeeRepository.IsUsernameTaken(_submittedUsername);

        var usernameIsGlados = _submittedUsername?.ToLower() == "glados";
        const string gladosEasterEggQuote = "Come to mommy. I made cake for you!";

        if (usernameExists)
        {
            Shared.Feedback = new CommandFeedback(
                usernameIsGlados ? gladosEasterEggQuote : "Username is valid.",
                CommandFeedback.FeedbackType.Success
            );
            _currentStage = Stage.PasswordInput;
        }
        else
        {
            Shared.Feedback = new CommandFeedback(
                usernameIsGlados ? gladosEasterEggQuote : "Employee with the submitted username doesn't exist.",
                CommandFeedback.FeedbackType.Error
            );
            _currentStage = Stage.UsernameInput;
        }
    }

    private void HandlePasswordInput()
    {
        _submittedPassword = ConsoleReader.ReadCommandFromUser();
        if (CheckForExitCommand(_submittedPassword)) return;
        _currentStage = Stage.LoginAttempt;
    }

    private void HandleLoginAttempt()
    {
        Shared.Feedback = new CommandFeedback("Authenticating...", CommandFeedback.FeedbackType.Loading);
        Shared.RefreshView();

        var result = Authenticator.Login(new LoginRequest(_submittedUsername ?? "", _submittedPassword ?? ""));

        switch (result)
        {
            case Authenticator.LoginResult.Success:
                _currentStage = Stage.LoginSuccess;
                Shared.Feedback = new CommandFeedback(
                    $"Employee {Session.Employee?.Name} {Session.Employee?.Surname} successfully logged in!",
                    CommandFeedback.FeedbackType.Success
                );
                Shared.View = new MessagingView();
                break;

            case Authenticator.LoginResult.UserDoesNotExist:
                Shared.Feedback = new CommandFeedback(
                    "Somehow, you don't exist anymore.", CommandFeedback.FeedbackType.Error
                );
                _currentStage = Stage.UsernameInput;
                break;

            case Authenticator.LoginResult.IncorrectPassword:
                Shared.Feedback = new CommandFeedback(
                    "Incorrect password.", CommandFeedback.FeedbackType.Error
                );
                _currentStage = Stage.PasswordInput;
                break;
        }
    }

    private string GetPrompt()
    {
        return _currentStage switch
        {
            Stage.UsernameInput => "Username:",
            Stage.PasswordInput => "Password:",
            _ => ""
        };
    }

    private bool CheckForExitCommand(string userInput)
    {
        var result = CommandProcessor.InvokeCommand(userInput, [new Exit()], true);
        if (result != CommandProcessor.Result.Success) return false;

        _currentStage = Stage.LoginAborted;
        return true;
    }
}