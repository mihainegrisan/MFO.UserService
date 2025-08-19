using FluentValidation;
using MFO.UserService.Application.DTOs;

namespace MFO.UserService.Application.Validators;

public class GetUserByEmailDtoValidator : AbstractValidator<GetUserByEmailDto>
{
    public GetUserByEmailDtoValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
    }
}