using ApertureMessenger.AlmsConnection;
using ApertureMessenger.AlmsConnection.Exceptions;
using ApertureMessenger.AlmsConnection.Objects;
using ApertureMessenger.AlmsConnection.Repositories;
using ApertureMessenger.UserInterface.Interfaces;
using ApertureMessenger.UserInterface.Messaging.Views;
using ApertureMessenger.UserInterface.Objects;

namespace ApertureMessenger.UserInterface.Messaging.Commands;

public class DirectMessage : ICommand
{
    public string[] Aliases { get; } = ["directmessage", "dm"];

    public void Invoke(string[] args)
    {
        if (args.Length == 0)
        {
            Shared.CommandResponse = new CommandResponse(
                "Missing argument: You must provide a username of the user to message.",
                CommandResponse.ResponseType.Error
            );
            return;
        }

        if (args.Length > 1)
        {
            Shared.CommandResponse = new CommandResponse(
                "Too many arguments for command.",
                CommandResponse.ResponseType.Error
            );
            return;
        }

        var username = args[0];

        if (username == Session.GetInstance().Employee?.Username)
        {
            Shared.CommandResponse = new CommandResponse(
                "You can't message yourself, you moron!",
                CommandResponse.ResponseType.Error
            );
            return;
        }

        Conversation conversation;
        try
        {
            conversation = ConversationRepository.GetDirectConversation(args[0]);
        }
        catch (EmployeeDoesNotExist)
        {
            Shared.CommandResponse = new CommandResponse(
                "The provided username doesn't exist.",
                CommandResponse.ResponseType.Error
            );
            return;
        }
        
        Shared.View = new ConversationView(conversation);
    }
}