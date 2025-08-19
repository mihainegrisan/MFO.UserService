using FluentValidation;
using MFO.UserService.Application.CommandsQueries.Commands;

namespace MFO.UserService.Application.Validators.Commands;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}