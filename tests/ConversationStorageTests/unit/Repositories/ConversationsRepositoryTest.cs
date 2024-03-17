using MongoDB.Driver;
using ConversationStorage.Interfaces;
using ConversationStorage.Repositories;
using ConversationStorage.Models;
using EphemeralMongo;
using Microsoft.Extensions.Configuration;

public class ConversationRepositoryTest : IClassFixture<ConversationRepositoryConfiguration>
{
    private IConversationRepository? _repository;

    public ConversationRepositoryTest(ConversationRepositoryConfiguration config)
    {
        _repository = config.repository;
    }

    [Fact]
    public async Task CreateConversation_WithNotExistingClient_ClientCreated()
    {
        var clientId = Guid.NewGuid();
        var conversation = new Conversation(
            Guid.NewGuid(), 
            ConversationStatus.Started, 
            DateTime.Now, "Telephone", []);

        var clientConversation = await _repository!.CreateConversation(clientId, conversation);

        Assert.IsType<Conversation>(clientConversation);
    }
}
public class ConversationRepositoryConfiguration : IDisposable
{
    private IMongoRunner? mongoDbRunner;
    private IMongoDatabase? _mongoDatabase;

    public IMongoClient? _mongoClient;
    public IConversationRepository? repository;
    private IConfiguration? _configuration;
    public ConversationRepositoryConfiguration()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").AddEnvironmentVariables().Build();
        var mongoDbEnv = _configuration["MONGO_DB_ENV"]??throw new NullReferenceException("Expecte mongoDbEnv");
        /* TESTING WITH OUT LOCAL DATABASE TO AVOID VERSIONING OF DEPENDENCIES */
        if (mongoDbEnv != "ci-cd")
        {
            _mongoClient = new MongoClient(_configuration.GetConnectionString("MONGO"));
            repository = new MongoConversationRepository(_mongoClient, _configuration);
            return;
        }
        /* CI - CD DATABASE CONFIGURATION ONLY UBUNT 20.04 FOR MONGO DEPENDENCY ON MEMORY */
        var options = new MongoRunnerOptions
        {
            UseSingleNodeReplicaSet = true,
            ConnectionTimeout = TimeSpan.FromSeconds(60),
            AdditionalArguments = "--quiet",
            MongoPort = 27017,
            KillMongoProcessesWhenCurrentProcessExits = true
        };
        mongoDbRunner = MongoRunner.Run(options);
        _mongoClient = new MongoClient(mongoDbRunner.ConnectionString);

    }


    public void Dispose()
    {
        var mongoDbEnv = _configuration!["MONGO_DB_ENV"];
        _mongoClient!.DropDatabase(_configuration.GetConnectionString("MONGO_DB_NAME"));

        if(mongoDbEnv == "ci-cd"){
        _mongoClient = null;
        mongoDbRunner!.Dispose();
        }
    }
}