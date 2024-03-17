using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class ValidateGuidParameterAttribute(string parameterName) : ActionFilterAttribute
{
    private readonly string _parameterName = parameterName;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ActionArguments.ContainsKey(_parameterName))
        {
            string? idString = context.ActionArguments[_parameterName] as string;

            if (!string.IsNullOrEmpty(idString))
            {
                if (!Guid.TryParse(idString, out _))
                {
                    context.Result = new BadRequestObjectResult($"Invalid GUID format for parameter {_parameterName}");
                    return;
                }
                // You may add additional validation if needed
            }
            else
            {
                context.Result = new BadRequestObjectResult($"GUID parameter {_parameterName} is required");
                return;
            }
        }
        else
        {
            context.Result = new BadRequestObjectResult($"GUID parameter {_parameterName} is missing");
            return;
        }

        base.OnActionExecuting(context);
    }
}
