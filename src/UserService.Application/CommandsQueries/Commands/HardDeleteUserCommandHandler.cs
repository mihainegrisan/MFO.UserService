using FluentResults;
using MediatR;
using UserService.Application.Interfaces;
using UserService.Domain.Errors;

namespace UserService.Application.CommandsQueries.Commands;

public sealed record HardDeleteUserCommand(Guid Id) : IRequest<Result>;

public class HardDeleteUserCommandHandler : IRequestHandler<HardDeleteUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public HardDeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(HardDeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Fail(new NotFoundError($"User with ID '{request.Id}' not found."));
        }

        var deleted = await _userRepository.DeleteAsync(user, cancellationToken);

        if (!deleted)
        {
            return Result.Fail("Failed to delete the user.");
        }

        return Result.Ok();
    }
}