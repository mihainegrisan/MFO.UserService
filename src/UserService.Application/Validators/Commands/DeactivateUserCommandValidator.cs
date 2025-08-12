using FluentValidation;
using UserService.Application.CommandsQueries.Commands;

namespace UserService.Application.Validators.Commands;

public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}