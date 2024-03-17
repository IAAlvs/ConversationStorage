using ConversationStorage.Dtos;
using ConversationStorage.Models;

namespace ConversationStorage.Interfaces;
public interface IConversationRepository{
    Task<Conversation> CreateConversation(Guid clientId, Conversation conversation);

    Task<Message?> AddMessage(Guid clientId, Guid conversationId, Message message);
    Task<Conversation?> PatchConversation(Guid clientId, Guid conversationId, PatchConversationDto PatchConversationDto);
    Task<Conversation?> GetConversation(Guid clientId, Guid conversationId);
}