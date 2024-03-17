using MongoDB.Driver;
using ConversationStorage.Interfaces;
using ConversationStorage.Repositories;
using ConversationStorage.Models;
using EphemeralMongo;
using Microsoft.Extensions.Configuration;

public class ConversationRepositoryTest : IClassFixture<ConversationRepositoryConfiguration>
{
    private IConversationRepository? _repository;

    public ConversationRepositoryTest()
    {}

    [Fact]
    public async Task CreateConversation_WithNotExistingClient_ClientCreated()
    {
        var clientId = Guid.NewGuid();
        var conversation = new Conversation(
            Guid.NewGuid(), 
            ConversationStatus.Started, 
            DateTime.Now, "Telephone", []);

        var clientConversation = await _repository!.CreateConversation(clientId, conversation);

        Assert.IsType<ClientConversation>(clientConversation);
    }
}
public class ConversationRepositoryConfiguration : IDisposable
{
    private IMongoRunner? mongoDbRunner;
    private IMongoClient? _mongoClient;
    private IMongoDatabase? _mongoDatabase;
    private IConversationRepository? _repository;
    private IConfiguration? _configuration;
    public ConversationRepositoryConfiguration()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();

        var options = new MongoRunnerOptions{
            UseSingleNodeReplicaSet = true, // Default: false
            StandardOuputLogger = line => Console.WriteLine(line), // Default: null
            StandardErrorLogger = line => Console.WriteLine(line),
            ConnectionTimeout = TimeSpan.FromSeconds(300),
            AdditionalArguments = "--quiet",
            MongoPort = 27017,
            KillMongoProcessesWhenCurrentProcessExits = true
        };
        mongoDbRunner = MongoRunner.Run(options);
        _mongoClient = new MongoClient(mongoDbRunner.ConnectionString);
        var databases = _mongoClient.ListDatabases();
        Console.WriteLine("============DATABASES ======");
        Console.WriteLine(databases);
        
        _repository = new MongoConversationRepository(_mongoClient, _configuration);
    }

    public void Dispose()
    {
        _mongoClient = null;
        mongoDbRunner!.Dispose();
    }
}