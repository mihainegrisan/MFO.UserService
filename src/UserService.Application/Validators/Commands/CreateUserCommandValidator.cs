using FluentValidation;
using UserService.Application.CommandsQueries.Commands;
using UserService.Application.Interfaces;

namespace UserService.Application.Validators.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(IUserRepository userRepository)
    {
        RuleFor(q => q.User.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
        RuleFor(q => q.User.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
        RuleFor(q => q.User.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
            .MustAsync(async (email, ct) => !await userRepository.ExistsByEmailAsync(email, ct)).WithMessage("Email must be unique.");
        RuleFor(q => q.User.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}