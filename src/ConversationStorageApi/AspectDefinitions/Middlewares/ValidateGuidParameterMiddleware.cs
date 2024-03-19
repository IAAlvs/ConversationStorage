using System.Net;
using ConversationStorage.Dtos;
namespace ConversationStorage.Middlewares;

public class ValidateGuidParameterMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var routeValues = context.Request.RouteValues;

        foreach (var routeValue in routeValues)
        {
            if (routeValue.Value is Guid)
                continue;

            if (routeValue.Key.ToLower().Contains("id", StringComparison.OrdinalIgnoreCase) && 
                Guid.TryParse(routeValue.Value!.ToString(), out _) == false)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = new List<ErrorMessageResponse> 
                { new (routeValue.Key, "Invalid GUID format", routeValue.Value) };
                await context.Response.WriteAsJsonAsync(errors);
                return;
            }
        }

        await _next(context);
    }
}

public static class ValidateGuidParameterMiddlewareExtensions
{
    public static IApplicationBuilder UseValidateGuidParameter(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidateGuidParameterMiddleware>();
    }
}
