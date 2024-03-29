using System.Net;
using ApertureMessenger.AlmsConnection.Exceptions;
using ApertureMessenger.AlmsConnection.Helpers;
using ApertureMessenger.AlmsConnection.Objects;
using ApertureMessenger.AlmsConnection.Requests;
using Newtonsoft.Json;

namespace ApertureMessenger.AlmsConnection.Repositories;

public static class ConversationRepository
{   
    public static void CreateGroupConversation(CreateGroupConversationRequest request)
    {
        var response = Connector.Post(
            "create-group-conversation",
            request.getRequestJson()
        );

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return;
            
            case HttpStatusCode.BadRequest:
                var errorResponse = ResponseParser.GetErrorResponse(response);
                switch (errorResponse.Message)
                {
                    case "EMPLOYEES DO NOT EXIST":
                        throw new EmployeesDoNotExist(errorResponse.Usernames?.ToArray() ?? Array.Empty<string>());
                }

                break;

            case HttpStatusCode.InternalServerError:
                throw new InternalAlmsError();
        }

        throw new UnhandledResponseError();
    }
    
    public static Conversation GetDirectConversation(string username)
    {
        var response = Connector.Get(
            "get-direct-conversation/" + username
        );

        var contentString = ResponseParser.GetResponseContent(response);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                Conversation? conversation;
                try
                {
                    conversation = JsonConvert.DeserializeObject<Conversation>(contentString);
                }
                catch (Exception)
                {
                    throw new JsonException("Failed parsing direct conversation JSON");
                }

                if (conversation == null)
                {
                    throw new JsonException("Direct conversation JSON was empty");
                }

                return conversation;

            case HttpStatusCode.NotFound:
                switch (ResponseParser.GetErrorResponse(response).Message)
                {
                    case "EMPLOYEE DOES NOT EXIST":
                        throw new EmployeeDoesNotExist();
                }

                break;
            
            case HttpStatusCode.InternalServerError:
                throw new InternalAlmsError();
        }

        throw new UnhandledResponseError();
    }
    
    public static Conversation[] GetAllConversations()
    {
        var response = Connector.Get(
            "get-all-conversations"
        );

        var contentString = ResponseParser.GetResponseContent(response);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                Conversation[]? conversations;
                try
                {
                    conversations = JsonConvert.DeserializeObject<Conversation[]>(contentString);
                }
                catch (Exception)
                {
                    throw new JsonException("Failed parsing conversations JSON");
                }

                if (conversations == null)
                {
                    throw new JsonException("Conversations JSON was empty");
                }

                return conversations;
        }

        throw new UnhandledResponseError();
    }
    
    public static Conversation[] GetDirectConversations()
    {
        var response = Connector.Get(
            "get-direct-conversations"
        );

        var contentString = ResponseParser.GetResponseContent(response);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                Conversation[]? conversations;
                try
                {
                    conversations = JsonConvert.DeserializeObject<Conversation[]>(contentString);
                }
                catch (Exception)
                {
                    throw new JsonException("Failed parsing direct conversations JSON");
                }

                if (conversations == null)
                {
                    throw new JsonException("Direct conversations JSON was empty");
                }

                return conversations;
        }

        throw new UnhandledResponseError();
    }
    
    public static Conversation[] GetGroupConversations()
    {
        var response = Connector.Get(
            "get-group-conversations"
        );

        var contentString = ResponseParser.GetResponseContent(response);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                Conversation[]? conversations;
                try
                {
                    conversations = JsonConvert.DeserializeObject<Conversation[]>(contentString);
                }
                catch (Exception)
                {
                    throw new JsonException("Failed parsing group conversations JSON");
                }

                if (conversations == null)
                {
                    throw new JsonException("Group conversations JSON was empty");
                }

                return conversations;
        }

        throw new UnhandledResponseError();
    }
}