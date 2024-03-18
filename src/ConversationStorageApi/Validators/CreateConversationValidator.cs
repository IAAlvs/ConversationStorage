using FluentValidation;
using ConversationStorage.Dtos;
using ConversationStorage.Interfaces;
namespace ConversationStorage.Validators;

public class IdentifierValidator :AbstractValidator<string>, IGlobalValidator{

    public IdentifierValidator(){
        RuleFor(x => x).MaximumLength(50).WithMessage("Identification max size is 50");
    }

    private bool BeValidGuid(string guid)
    {
        try{
            bool isValid = Guid.TryParse(guid, out _);
            return isValid;
        }
        catch{
            return false;
        }
        
    }

}


public class CreateConversationDtoValidator : AbstractValidator<CreateConversationDto>, IGlobalValidator
{
    public CreateConversationDtoValidator ()
    {
        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Origin must not be empty")
            .MaximumLength(50).WithMessage("Name max length are 50 characters")
            .MinimumLength(2).WithMessage("Name min length are 3 characters");  
    }
}
