using System.Text.Json;
namespace ConversationStorage.Middlewares;
public class JsonExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public JsonExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var result = JsonSerializer.Serialize(new { error = "An error occurred while processing your json body request." });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
        return context.Response.WriteAsync(result);
    }
}

public static class JsonExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseJsonExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JsonExceptionMiddleware>();
    }
}
