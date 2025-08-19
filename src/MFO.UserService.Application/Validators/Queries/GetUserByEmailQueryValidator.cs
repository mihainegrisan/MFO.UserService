using FluentValidation;
using MFO.UserService.Application.CommandsQueries.Queries;

namespace MFO.UserService.Application.Validators.Queries;

public class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUserByEmailQueryValidator()
    {
        RuleFor(q => q.User.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
    }
}