using ConversationStorage.Dtos;
namespace ConversationStorage.Services;

interface IConversationService{
    Task<ConversationDto> CreateConversation(Guid clientId, CreateConversationDto dto);
    Task<ConversationDto> PatchConversation(Guid clientId, Guid conversationId, PatchConversationDto dto);
    Task<MessagesDto> AddMessage(Guid clientId, Guid conversationId, AddMessageRequestDto dto);
    Task<ConversationDto?> RetrieveConversation(Guid clientId, Guid conversationId);
}