using MongoDB.Driver;
using ConversationStorage.Interfaces;
using ConversationStorage.Repositories;
using ConversationStorage.Models;
using EphemeralMongo;
using Microsoft.Extensions.Configuration;

public class ConversationRepositoryTest : IDisposable
{
    private readonly IConversationRepository? _repository;
    private IMongoRunner? mongoDbRunner;
    public IMongoClient? _mongoClient;
    public IConversationRepository? repository;
    private IConfiguration? _configuration;

    public ConversationRepositoryTest()
    {
        Console.WriteLine("REPOSITORY CTO===================================");
        Console.WriteLine(_repository);
                _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");      
        /* TESTING WITH OUT LOCAL DATABASE TO AVOID VERSIONING OF DEPENDENCIES */
        if(environment == "ci-cd"){
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
            return;
        }
            Console.WriteLine(_configuration.GetConnectionString("MONGO_DB_NAME"));
            _mongoClient = new MongoClient(_configuration.GetConnectionString("MONGO"));
            repository = new MongoConversationRepository(_mongoClient, _configuration);
            return;   
    }
    public void Dispose()
    {
        var mongoDbEnv = _configuration!["MONGO_DB_ENV"];
        if(mongoDbEnv == "ci-cd"){
        _mongoClient!.DropDatabase(_configuration.GetConnectionString("MONGO_DB_NAME"));
        _mongoClient = null;
        mongoDbRunner!.Dispose();
        }
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
        Console.WriteLine("REPOSITORY TEST===================================");
        Console.WriteLine(_repository);

        Assert.IsType<Conversation>(clientConversation);
    }
}
/* public class ConversationRepositoryConfiguration : IDisposable
{
    private IMongoRunner? mongoDbRunner;
    public IMongoClient? _mongoClient;
    public IConversationRepository? repository;
    private IConfiguration? _configuration;
    public ConversationRepositoryConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");      
        if(environment == "ci-cd"){
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
            return;
        }
            Console.WriteLine(_configuration.GetConnectionString("MONGO_DB_NAME"));
            _mongoClient = new MongoClient(_configuration.GetConnectionString("MONGO"));
            repository = new MongoConversationRepository(_mongoClient, _configuration);
            return;   


    }


    public void Dispose()
    {
        var mongoDbEnv = _configuration!["MONGO_DB_ENV"];
        if(mongoDbEnv == "ci-cd"){
        _mongoClient!.DropDatabase(_configuration.GetConnectionString("MONGO_DB_NAME"));
        _mongoClient = null;
        mongoDbRunner!.Dispose();
        }
    }
} */