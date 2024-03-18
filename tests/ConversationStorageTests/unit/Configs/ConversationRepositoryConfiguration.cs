using MongoDB.Driver;
using ConversationStorage.Interfaces;
using ConversationStorage.Repositories;
using EphemeralMongo;
using Microsoft.Extensions.Configuration;
using ConversationStorage.Models;

namespace ConversationStorageTests.Configurations;


interface IConversationRepositoryConfiguration:IDisposable{

}
public class ConversationRepositoryConfiguration : IConversationRepositoryConfiguration
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
            repository = new MongoConversationRepository(_mongoClient, _configuration);
            return;
        }
            _mongoClient = new MongoClient(_configuration.GetConnectionString("MONGO"));
            repository = new MongoConversationRepository(_mongoClient, _configuration);
            return;   


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