using FluentResults;
using FluentValidation;
using MediatR;

namespace UserService.API.Middlewares;

public class ValidationBehaviorMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TResponse : ResultBase, new ()
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviorMiddleware(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var result = new TResponse();
            foreach (var failure in failures)
                result.Reasons.Add(new Error(failure.ErrorMessage));

            return result;
        }

        return await next(cancellationToken);
    }
}