
using ConversationStorage.Dtos;
using ConversationStorage.Models;
using ConversationStorage.Interfaces;
namespace ConversationStorage.Services;
public class ConversationService(IConversationRepository repository) : IConversationService
{
    private readonly IConversationRepository _repository = repository;

    public async Task<MessagesDto> AddMessage(Guid clientId, Guid conversationId, AddMessageRequestDto dto)
    {
        var message = Message.FromDto(dto);
        var conversation = await _repository.AddMessage(clientId, conversationId, message);
        return conversation switch
        {
            null => throw new NullReferenceException($"Not found conversation with id {conversationId} to add message"),
            _ => conversation
        };
    }

    public async Task<ConversationDto> CreateConversation(Guid clientId, CreateConversationDto dto)
    {
        var conversation = Conversation.FromDto(dto);
        return await _repository.CreateConversation(clientId, conversation);
    }

    public async Task<ConversationDto> PatchConversation(Guid clientId,Guid conversationId, PatchConversationDto dto)
    {
        var conversation = await _repository.PatchConversation(clientId, conversationId, dto);
        return conversation.ToDto();
    }

    public async Task<ConversationDto?> RetrieveConversation(Guid clientId, Guid conversationId)
    {
        var conversation = await _repository.GetConversation(clientId, conversationId);
        return conversation switch
        {
            null => null,
            _ => conversation.ToDto(),
        };
    }
}