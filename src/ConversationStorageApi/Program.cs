using ConversationStorage.Endpoints;
using Serilog;
using ConversationStorage.AspectDefinitions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;


internal partial class Program{ 
    private static void Main(string[] args){
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));


        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
        Log.Information("Starting up"); 
        try{
            var builder = WebApplication.CreateBuilder(args);
            
            SerilogAspectDefinition.DefineAspect(builder);
            ValidationAspectDefinition.DefineAspect(builder.Services, builder.Configuration);
            SwaggerEndpointDefinition.DefineServices(builder.Services);
            ConversationStorageEndpoints.DefineServices(builder.Services);
            
            var app = builder.Build();

            SerilogAspectDefinition.ConfigureAspect(app);
            SwaggerEndpointDefinition.DefineEndpoints(app);
            ConversationStorageEndpoints.DefineEndpoints(app);

            app.Run();

        }
        catch (Exception ex)
        {
            var type = ex.GetType().Name;
            if (type.Equals("HostAbortedException", StringComparison.Ordinal)) throw;
            if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }
}



