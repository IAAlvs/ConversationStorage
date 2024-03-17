using ConversationStorage.Models;
namespace ConversationStorage.Dtos;

public record AddMessageRequestDto(MessageType Type, ConversationStatus Status, string Text, MessageMetadata? Metadata );
public record MessagesDto(string Type, string Status, DateTime Timestamp, string TextMessage);
public record ConversationDto(Guid Id, string ConversationStatus, string Origin, List<MessagesDto> Messages);
public record CreateConversationDto(string Origin);
public record PatchConversationDto(ConversationStatus? Status, string? Origin);