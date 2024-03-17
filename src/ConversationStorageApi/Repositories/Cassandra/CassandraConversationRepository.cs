/* using ConversationStorage.Interfaces;

namespace ConversationStorage.Repositories;

class CassandraConversationRepository(ICassandraDbContext context) : IConversationRepository
{
    private readonly ICassandraDbContext _context = context;

    public Task<object> AddMessage(string conversationId, object AddMessageDto)
    {
        //string tableName = "messages";
        //string query = $"INSERT into {tableName} (id, type, )"; 
        throw new NotImplementedException();

    }

    public Task<object> GetConversation(string conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<object> GetFullConversation(string conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<object> PatchConversation(string conversationId, object PatchConversationDto)
    {
        throw new NotImplementedException();
    }
} */