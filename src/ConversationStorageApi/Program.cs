using ConversationStorage.Endpoints;
using Serilog;
using ConversationStorage.AspectDefinitions;
using ConversationStorage.Middlewares;

internal partial class Program{ 
    private static void Main(string[] args){


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
            app.UseValidateGuidParameter();
            app.UseJsonExceptionHandler();
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



