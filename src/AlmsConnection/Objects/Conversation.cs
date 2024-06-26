using Newtonsoft.Json;

namespace ApertureMessenger.AlmsConnection.Objects;

[Serializable]
public class Conversation
{
    [JsonProperty("id")] public int Id;

    [JsonProperty("name")] public string? Name;

    [JsonProperty("isGroup")] public bool IsGroup;

    [JsonProperty("datetimeCreated")] public DateTime DateTimeCreated;

    [JsonProperty("datetimeUpdated")] public DateTime DateTimeUpdated;

    [JsonProperty("participants")] public List<Employee>? Participants;

    [JsonProperty("unreadMessages")] public int? UnreadMessageCount;

    [JsonConstructor]
    public Conversation(
        int id,
        string? name,
        bool isGroup,
        DateTime dateTimeCreated,
        DateTime dateTimeUpdated,
        List<Employee>? participants,
        int? unreadMessageCount
    )
    {
        Id = id;
        Name = name;
        IsGroup = isGroup;
        DateTimeCreated = dateTimeCreated;
        DateTimeUpdated = dateTimeUpdated;
        Participants = participants;
        UnreadMessageCount = unreadMessageCount;
    }

    public override string ToString()
    {
        var participantList =
            Participants != null ? string.Join(", ", Participants.ConvertAll(p => p.Username)) : "null";

        return $"Conversation ID: {Id}\n" +
               $"Name: {Name ?? "null"}\n" +
               $"Is Group: {IsGroup}\n" +
               $"Date Time Created: {DateTimeCreated}\n" +
               $"Date Time Updated: {DateTimeUpdated}\n" +
               $"Participants: {participantList}" +
               $"Unread Messages: {UnreadMessageCount}";
    }
}