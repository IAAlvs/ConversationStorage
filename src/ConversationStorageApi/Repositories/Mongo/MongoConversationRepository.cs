using ConversationStorage.Dtos;
using ConversationStorage.Interfaces;
using ConversationStorage.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ConversationStorage.Repositories;

public class MongoConversationRepository : IConversationRepository{
    private readonly IMongoCollection<ClientConversation> _conversations;
    private readonly IMongoClient _mongoClient;
    private readonly IConfiguration _config;

    public MongoConversationRepository(IMongoClient client, IConfiguration config)
    {   _config = config;
        _mongoClient = client;
        var database = _mongoClient!.GetDatabase(_config.GetConnectionString("MONGO_DB_NAME"));
        _conversations = database.GetCollection<ClientConversation>("conversations");
    }
    public async Task<MessagesDto?> AddMessage(Guid clientId, Guid conversationId, Message message)
    {
        var filter = 
        Builders<ClientConversation>.Filter.Eq(c => c.Id, clientId) &
        Builders<ClientConversation>.Filter.ElemMatch(
            c => c.Conversations,
            Builders<Conversation>.Filter.Eq(c => c.Id, conversationId)
        );
        //Console.WriteLine(JsonSerializer.Serialize(message));
        var update = Builders<ClientConversation>.Update.Push(x => x.Conversations.FirstMatchingElement().Messages,  message);

        await _conversations.UpdateOneAsync(filter, update);

        //var update = Builders<Conversation>.Update.Push("messages", message);
        return message.ToDto();
    }
    public async Task<ConversationDto> CreateConversation(Guid clientId, Conversation conversation)
    {
        var filter = Builders<ClientConversation>.Filter.Eq(x => x.Id, clientId);
        var update = Builders<ClientConversation>.Update.Push(x => x.Conversations, conversation);
        
        var options = new FindOneAndUpdateOptions<ClientConversation>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };    
        var clientConversation = await _conversations.FindOneAndUpdateAsync(filter, update, options);
        return conversation.ToDto();
    }
    /* 
        public async Task<ConversationDto> CreateConversation(Guid clientId, Conversation conversation)
    {
        var filter = Builders<ClientConversation>.Filter.Eq(x => x.Id, clientId);
        var conversations = await _conversations.FindAsync(Builders<ClientConversation>.Filter.Empty);
        var clientConversation = await _conversations.Find(filter).FirstOrDefaultAsync();
        if (clientConversation == null)
        {
            clientConversation = new ClientConversation(clientId);
            clientConversation.Conversations.Add(conversation);

            await _conversations.InsertOneAsync(clientConversation);
            return conversation.ToDto();
        }
        var update = Builders<ClientConversation>.Update.Push(x => x.Conversations, conversation);

        await _conversations.UpdateOneAsync(filter, update);
        return conversation.ToDto();

    }
    */


    public async Task<Conversation?> GetConversation(Guid clientId, Guid conversationId)
    {
        var filter = 
        Builders<ClientConversation>.Filter.Eq(c => c.Id, clientId) &
        Builders<ClientConversation>.Filter.ElemMatch(
            c => c.Conversations,
            Builders<Conversation>.Filter.Eq(c => c.Id, conversationId)
        );
        var conversation = await _conversations.Find(filter).FirstOrDefaultAsync();
        return conversation.Conversations.Find(c => c.Id ==conversationId);

    }

    public async Task<Conversation> PatchConversation(Guid clientId, Guid conversationId, PatchConversationDto patchDto)
    {
        var filter = Builders<ClientConversation>.Filter.And(
            Builders<ClientConversation>.Filter.Eq(c => c.Id, clientId),
            Builders<ClientConversation>.Filter.ElemMatch(
                c => c.Conversations,
                Builders<Conversation>.Filter.Eq(c => c.Id, conversationId)
            )
        );
        var updateDefinitions = patchDto.GetType().GetProperties()
            .Where(p => p.GetValue(patchDto) != null)
            .Select(p =>
            {
                var propertyName = p.Name;
                var propertyValue = p.GetValue(patchDto);
                return Builders<ClientConversation>.Update.Set($"Conversations.$.{propertyName}", propertyValue);
            });
        var combinedUpdate = Builders<ClientConversation>.Update.Combine(updateDefinitions);
        var options = new FindOneAndUpdateOptions<ClientConversation, ClientConversation>
        {
            ReturnDocument = ReturnDocument.After
        };
        var updatedClientConversation = await _conversations.FindOneAndUpdateAsync(
            filter,
            combinedUpdate,
            options
        ) ?? throw new Exception("Conversation not found");
        return updatedClientConversation.Conversations.Find(c => c.Id == conversationId)
        ??throw new Exception("Conversation not found");
;
    }


}