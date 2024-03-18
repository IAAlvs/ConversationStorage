using FluentValidation;
using ConversationStorage.Dtos;
using ConversationStorage.Interfaces;
namespace ConversationStorage.Validators;



public class PatchConversationDtoValidator : AbstractValidator<PatchConversationDto>, IGlobalValidator
{
    public PatchConversationDtoValidator ()
    {
        RuleFor(x => x.Origin)
            .MaximumLength(50).WithMessage("Name max length are 50 characters")
            .MinimumLength(2).WithMessage("Name min length are 3 characters");  
        RuleFor(x => x.Status).IsInEnum().WithMessage("Parameter status should be a valid status choice");
    }
}
