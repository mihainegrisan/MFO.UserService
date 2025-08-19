using FluentResults;
using MediatR;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public sealed record DeleteUserCommand(Guid Id) : IRequest<Result>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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