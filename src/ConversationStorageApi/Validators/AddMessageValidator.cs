using FluentValidation;
using ConversationStorage.Dtos;
using ConversationStorage.Interfaces;
using System.Text.RegularExpressions;
using ConversationStorage.Models;
namespace ConversationStorage.Validators;



public class AddMessageDtoValidator : AbstractValidator<AddMessageRequestDto>, IGlobalValidator
{
    public AddMessageDtoValidator ()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Parameter should be a valid status choice");
        RuleFor(x => x.Text).MaximumLength(200).MinimumLength(0).NotNull().NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Metadata).SetValidator(new MetadataValidator()!).When(x => x.Metadata != null);
    }
    /* Must(x => !ContainsSql(x)).WithMessage("Some standar of message where not fully used"); */
    private bool ContainsSql(string parametro)
    {
        // Expresión regular para detectar patrones SQL comunes
        string sqlPattern = @"(\b(?:SELECT|UPDATE|DELETE|INSERT|DROP|ALTER|CREATE|EXEC|UNION|HAVING|ORDER BY|GROUP BY|FROM|WHERE)\b)";

        // Verificar si el parámetro contiene algún patrón SQL
        return Regex.IsMatch(parametro, sqlPattern, RegexOptions.IgnoreCase);
    }
}
public class MetadataValidator : AbstractValidator<MessageMetadata>
{
    public  MetadataValidator()
    {
        RuleFor(x => x.FileVoiceId);
        RuleFor(x => x.AditionalParams).MaximumLength(2000).When(x => !String.IsNullOrEmpty(x.AditionalParams));

    }
}
