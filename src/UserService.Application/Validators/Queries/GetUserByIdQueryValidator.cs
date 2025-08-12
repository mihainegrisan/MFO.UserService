using FluentValidation;
using UserService.Application.CommandsQueries.Queries;

namespace UserService.Application.Validators.Queries;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}