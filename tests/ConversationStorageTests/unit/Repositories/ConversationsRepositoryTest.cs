using MongoDB.Driver;
using ConversationStorage.Interfaces;
using ConversationStorage.Models;
using ConversationStorageTests.Configurations;
using ConversationStorage.Dtos;

public class ConversationRepositoryTest : IClassFixture<ConversationRepositoryConfiguration>
{
    private readonly IConversationRepository _repository;
    public IMongoClient? _mongoClient;

    public ConversationRepositoryTest(ConversationRepositoryConfiguration config)
    {
        _repository = config.repository?? throw new NullReferenceException("Expected definition of repository");
    }

    [Fact]
    public async Task CreateConversation_WithNotExistingClient_ClientCreated()
    {
        var clientId = Guid.NewGuid();
        var conversation = new Conversation(
            Guid.NewGuid(), 
            ConversationStatus.Started, 
            DateTime.UtcNow, "Telephone", []);
        TimeSpan tolerance = TimeSpan.FromMilliseconds(1);


        var clientConversationInserted = await _repository!.CreateConversation(clientId, conversation);
        
        var clientCRecover = await _repository.GetConversation(clientId, conversation.Id);
        Assert.IsType<Conversation>(clientConversationInserted);
        Assert.NotNull(clientCRecover);
        Assert.Equal(ConversationStatus.Started, clientCRecover.Status);
        Assert.Equal(clientConversationInserted.Origin, clientCRecover.Origin);
        Assert.InRange(clientConversationInserted.DateInit, clientCRecover.DateInit - tolerance, clientCRecover.DateInit + tolerance);
        Assert.Equal(clientConversationInserted.Id, clientCRecover.Id);
    }
    [Fact]
    public async Task RetrieveConversation_NotExisting_Null()
    {
        var clientId = Guid.NewGuid();

        var clientConversation = await _repository!.GetConversation(clientId, Guid.NewGuid());
        
        Assert.Null(clientConversation);
    }
    [Fact]
    public async Task PatchConversation_ConversationPatched()
    {
        var clientId = Guid.NewGuid();
        var conversation = new Conversation(
            Guid.NewGuid(), 
            ConversationStatus.Started, 
            DateTime.UtcNow, "Telephone", []);
        var conversationPatch = new PatchConversationDto(ConversationStatus.Progress, "Whatsapp");

        var conversationCreated = await _repository!.CreateConversation(clientId, conversation);

        Assert.Equal(ConversationStatus.Started,conversationCreated.Status);
        Assert.Equal("Telephone", conversationCreated.Origin);

        var conversationPatched = await _repository!.PatchConversation(clientId, conversationCreated.Id, conversationPatch);
        var clientCRecover = await _repository!.GetConversation(clientId, conversation.Id);

        Assert.Equal(conversationPatch.Status, clientCRecover!.Status);
        Assert.Equal(conversationPatch.Origin, clientCRecover.Origin);
    }
    [Fact]
    public async Task AddMessage_ConversationNotExist_Null()
    {

        var clientId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var message = new Message(Guid.NewGuid(), 
        MessageType.UserRequest, 
        DateTime.UtcNow,ConversationStatus.Progress, 
        "Hello world",
        null);

        var messageToAdd = await _repository.AddMessage(clientId, conversationId, message);

        Assert.Null(messageToAdd);
    }
        [Fact]
    public async Task AddMessage_Conversation_ConversationWithMessage()
    {

        var clientId = Guid.NewGuid();
        var message = new Message(Guid.NewGuid(), 
        MessageType.UserRequest, 
        DateTime.UtcNow,ConversationStatus.Progress, 
        "Hello world",
        null);
        var conversation = new Conversation(
            Guid.NewGuid(), 
            ConversationStatus.Started, 
            DateTime.UtcNow, "Telephone", []);
        TimeSpan tolerance = TimeSpan.FromMilliseconds(1);


        await _repository!.CreateConversation(clientId, conversation);
        var messageToAdd = await _repository.AddMessage(clientId, conversation.Id, message);
        var conversationRecover = await _repository!.GetConversation(clientId, conversation.Id);

        //Assert.Contains(messageToAdd, conversationRecover!.Messages);
        //Assert.Equivalent(conversationRecover!.Messages[0], messageToAdd);
        Assert.Equal(message.Id, conversationRecover!.Messages[0].Id);
        Assert.Equal(message.Type, conversationRecover.Messages[0].Type);
        Assert.Equal(message.StatusInConversation, conversationRecover.Messages[0].StatusInConversation);
        Assert.InRange(message.Timestamp, conversationRecover.Messages[0].Timestamp - tolerance, 
        conversationRecover.Messages[0].Timestamp + tolerance);

        Assert.Equal(message.Text, conversationRecover.Messages[0].Text);
        Assert.Equal(message.Metadata, conversationRecover.Messages[0].Metadata);

    }
}

