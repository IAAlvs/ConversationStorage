using ConversationStorage.Dtos;
using ConversationStorage.Interfaces;
using ConversationStorage.Repositories;
using ConversationStorage.Services;
using ConversationStorage.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ConversationStorage.Endpoints;
public class ConversationStorageEndpoints
{
    public const string API_VERSION = "v1";

    public static void DefineServices(IServiceCollection services)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        services.AddScoped<IMongoClient, MongoClient>(_ => new MongoClient(config.GetConnectionString("MONGO")));
        services.AddScoped<IConversationRepository, MongoConversationRepository>();
        services.AddScoped<IConversationService, ConversationService>();
    }
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        //Create conversation 
        app.MapPost("/api/"+ API_VERSION+"/conversations/{clientId}", CreateConversation)  
            .WithName("Created a  conversation");
        //Add a message to conversation
        app.MapPost("/api/"+ API_VERSION+"/{clientId}/messages/{conversationId}", AddMessage)  
            .WithName("Adds a message to a conversation");
        app.MapGet("/api/"+ API_VERSION+"/{clientId}/messages/{conversationId}", RetrieveConversation)  
            .WithName("Get conversation and all their messages");
        app.MapPatch("/api/"+ API_VERSION+"/{clientId}/conversations/{conversationId}", PatchConversation).
            WithName("Updates conversation status and other features"); 
    }
    [ValidateGuidParameter("clientId")]

    private static async Task<IResult> CreateConversation(
        [FromRoute(Name = "clientId")] Guid clientId,
        IConversationService conversationService, 
        CreateConversationDto createConversationDto,
        IValidator<CreateConversationDto> _validator)
    {
        var validate = _validator.Validate(createConversationDto);
        if(!validate.IsValid)
            return Results.BadRequest(validate.Errors);
        return await conversationService.CreateConversation(clientId, createConversationDto)
        is { } conversation 
        ? Results.Ok(conversation)
        : Results.NotFound(); 
    }

    [ValidateGuidParameter("clientId")]
    [ValidateGuidParameter("conversationId")]


    private static async Task<IResult> PatchConversation(
        IConversationService conversationService,
        [FromRoute(Name = "clientId")] Guid clientId,
        [FromRoute(Name = "conversationId")] Guid conversationId, 
        PatchConversationDto patchDto,
        IValidator<PatchConversationDto> _validator)
    {
        try
        {
            var validate = _validator.Validate(patchDto);
            if(!validate.IsValid)
                return Results.BadRequest(validate.Errors);
            return await conversationService.PatchConversation(clientId, conversationId, patchDto )
            is { } conversationPatched 
            ? Results.Ok(conversationPatched)
            : Results.NotFound();
        }

        catch(Exception ex){
            return ex switch {
                NullReferenceException e => Results.NotFound(e.Message),
                Exception => Results.Problem() // Manejar otros tipos de excepción
            };
        }
    }
    [ValidateGuidParameter("clientId")]
    [ValidateGuidParameter("conversationId")]
    private static async Task<IResult> RetrieveConversation(
        IConversationService conversationService,
        [FromRoute(Name = "clientId")] Guid clientId,
        [FromRoute(Name = "conversationId")] Guid conversationId)
    {
        return await conversationService.RetrieveConversation(clientId,conversationId)
        is { } conversation 
        ? Results.Ok(conversation)
        : Results.NotFound();     }

/*     private static async Task<IResult> ListMessages(HttpContext context)
    {
        throw new NotImplementedException();
    } */
    [ValidateGuidParameter("clientId")]
    [ValidateGuidParameter("conversationId")]

    private static async Task<IResult> AddMessage(
        IConversationService conversationService,
        [FromRoute(Name = "clientId")] Guid clientId,
        [FromRoute(Name = "conversationId")] Guid conversationId, 
        AddMessageRequestDto messageDto,
        
        IValidator<AddMessageRequestDto> validator)
    {
        try{
            var validate = validator.Validate(messageDto);

            if(!validate.IsValid)
                return Results.BadRequest(validate.Errors);
            return await conversationService.AddMessage(clientId, conversationId, messageDto)
            is { } conversation 
            ? Results.Ok(conversation)
            : Results.NotFound(); 
        }
        catch(Exception ex){
            return ex switch {
                NullReferenceException e => Results.NotFound(e.Message),
                Exception => Results.Problem() // Manejar otros tipos de excepción
        };

        }

    }
}
