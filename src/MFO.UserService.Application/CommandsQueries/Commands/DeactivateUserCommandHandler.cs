using AutoMapper;
using FluentResults;
using MediatR;
using MFO.Contracts.User.DTOs;
using MFO.UserService.Application.Interfaces;
using MFO.UserService.Domain.Errors;

namespace MFO.UserService.Application.CommandsQueries.Commands;

public record DeactivateUserCommand(Guid Id) : IRequest<Result<UserDto>>;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public DeactivateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Result.Fail<UserDto>(new NotFoundError($"User with ID '{request.Id}' not found."));
        }

        var deactivated = await _userRepository.SetUserActiveStateAsync(user, false, cancellationToken);
        if (!deactivated)
        {
            return Result.Fail($"Failed to deactivate user with ID '{request.Id}'.");
        }

        var userDto = _mapper.Map<UserDto>(deactivated);

        return Result.Ok(userDto);
    }
}