using FluentValidation;
using UserService.Application.CommandsQueries.Commands;

namespace UserService.Application.Validators.Commands;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}