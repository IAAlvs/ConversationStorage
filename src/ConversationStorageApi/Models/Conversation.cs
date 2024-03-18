using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using ConversationStorage.Dtos;
namespace ConversationStorage.Models;

public enum ConversationStatus
{
    Started,
    Progress,
    FailedHumanFinished,
    AskedForHumanHelp,
    FailedModelFinished,
    Finished
}
public enum MessageType{
    UserRequest,
    UserResponse,
    MachineRequest,
    MachineResponse
}
public record MessageMetadata(Guid FileVoiceId, string? AditionalParams);

public class Message(Guid id, MessageType type, DateTime timestamp, ConversationStatus status, string text, MessageMetadata?  metadata)
{
    [BsonId]

    public Guid Id { get;set; } = id;
    public MessageType Type { get;set; } = type;
    public ConversationStatus StatusInConversation { get;set; } = status;
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Timestamp { get;set; } = timestamp;
    public String Text { get;set; } = text;
    [BsonIgnoreIfNull]
    public MessageMetadata? Metadata { get;set; } = metadata;
    /* Metadata expected to have or not have */
    /* Refers to the file id saves in blobstorage 
        Refers to dispotive origin MML or Human telephone 
    String? FileVoiceId { get; } = fileVoiceId;
    */
    public static Message FromDto(AddMessageRequestDto messageDto){
        var(type, status, text, metadata) = messageDto;
        return new(Guid.NewGuid(), type, DateTime.UtcNow, status, text, metadata );
    }
    public MessagesDto ToDto() => new(Type.ToString(), StatusInConversation.ToString(), Timestamp, Text);

}

public class Conversation(Guid id, ConversationStatus status, DateTime dateInit, string origin, List<Message>? messages)
{    
    [BsonId]
   //[BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get;  } = id;
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]

    public DateTime DateInit { get; } = dateInit;
    public ConversationStatus Status { get; } = status;
    /* Originally empty */
    /* Refers to a kind of conversation telephonical or message */
    public String Origin { get; } = origin;
    public List<Message> Messages { get; } = messages?? [];
    public static Conversation FromDto(CreateConversationDto conversationDto){
        var origin = conversationDto.Origin;
        return new(Guid.NewGuid(), ConversationStatus.Started, DateTime.UtcNow, origin, []);
    }
    public ConversationDto ToDto() => new (Id, Status.ToString(), Origin, Messages.Select(m => m.ToDto()).ToList());
}

public class ClientConversation(Guid clientId){
    [BsonId]
    //[BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; } = clientId;
    public List<Conversation> Conversations { get;set; } = []; 

}